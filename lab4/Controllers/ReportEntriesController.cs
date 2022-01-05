using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.DataManager.Exceptions;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
[Route("[controller]")]
public class ReportEntriesController : BaseController
{
    private ILogger<ReportEntriesController> _logger;

    public ReportEntriesController(IDataManager dataManager, IMapper mapper, ILogger<ReportEntriesController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult Show(int id)
    {
        var reportEntry = DataManager.FindReportEntryById(id, x => x
            .Include(y => y.Category));
        if (reportEntry == null)
            return NotFound();
        return Ok(Mapper.Map<ReportEntryModel>(reportEntry));
    }

    [HttpPatch]
    [Route("{id}")]
    public IActionResult Edit(int id, [FromBody] ReportEntryModel reportEntry)
    {
        var updatedReportEntry = DataManager.FindReportEntryById(id, x => x
            .Include(y => y.Report));
        if (updatedReportEntry == null)
            return NotFound();
        if (updatedReportEntry.Report!.Frozen)
            return Forbid();
        var category = DataManager.FindCategoryByProjectCodeAndCode(reportEntry.Code, reportEntry.Subcode);
        updatedReportEntry.CategoryCode = category?.Code;
        updatedReportEntry.Time = reportEntry.Time;
        updatedReportEntry.Description = reportEntry.Description ?? "";
        updatedReportEntry.Timestamp = reportEntry.Timestamp;
        try
        {
            DataManager.UpdateReportEntry(updatedReportEntry);
            return Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.Clear();
            var newTimestamp = DataManager.FindReportEntryById(id)!.Timestamp;
            var returnModel = Mapper.Map<ReportEntryForEditingModel>(updatedReportEntry);
            returnModel.Timestamp = newTimestamp;
            return Conflict(returnModel);
        }
    }

    [HttpPost]
    public IActionResult Add(ReportEntryModel reportEntry)
    {
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, reportEntry.Date);
        if (report.Frozen)
            return Forbid();
        var mappedReport = Mapper.Map<ReportEntry>(reportEntry);
        mappedReport.ReportId = report.Id;
        var project = DataManager.FindProjectByCode(reportEntry.Code);
        switch (project)
        {
            case null:
                ModelState.AddModelError(nameof(reportEntry.Code), ErrorMessages.GetProjectNotFoundMessage(reportEntry.Code));
                break;
            case { Active: false }:
                ModelState.AddModelError(nameof(reportEntry.Code), ErrorMessages.GetProjectNoLongerActiveMessage(reportEntry.Code));
                break;
        }
        if (!ModelState.IsValid)
            goto InvalidModelState;
        mappedReport.ProjectCode = project!.Code;
        if (!string.IsNullOrEmpty(reportEntry.Subcode))
        {
            var category = DataManager.FindCategoryByProjectCodeAndCode(reportEntry.Code, reportEntry.Subcode);
            if (category != null)
                mappedReport.CategoryCode = category.Code;
            else
                ModelState.AddModelError(nameof(reportEntry.Code), ErrorMessages.GetCategoryNotFoundMessage(reportEntry.Code, reportEntry.Subcode));
        }
        if (!ModelState.IsValid)
            goto InvalidModelState;
        if (!string.IsNullOrEmpty(reportEntry.Subcode))
        {
            var category = DataManager.FindCategoryByProjectCodeAndCode(project!.Code, reportEntry.Subcode);
            if (category != null)
                mappedReport.CategoryCode = category.Code;
            else
                ModelState.AddModelError(nameof(reportEntry.Subcode), ErrorMessages.GetCategoryNotFoundMessage(reportEntry.Subcode, reportEntry.Code));
        }
        if (!ModelState.IsValid)
            goto InvalidModelState;

        DataManager.AddReportEntry(mappedReport);
        return Ok();

        InvalidModelState:
        var addingModel = Mapper.Map<ReportEntryForAddingModel>(reportEntry);
        return BadRequest(addingModel);
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            DataManager.DeleteReportEntryById(id);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        return Ok();
    }
}
