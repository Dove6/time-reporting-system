using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.DataManager.Exceptions;
using Trs.Extensions;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
public class ReportEntryController : BaseController
{
    private ILogger<ReportEntryController> _logger;

    public ReportEntryController(IDataManager dataManager, IMapper mapper, ILogger<ReportEntryController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    public IActionResult Show(int id)
    {
        var reportEntry = DataManager.FindReportEntryById(id, x => x
            .Include(y => y.Category));
        if (reportEntry == null)
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
        return View(Mapper.Map<ReportEntryModel>(reportEntry));
    }

    private void FillSelectListsInEditingModel(ReportEntryForEditingModel editingModel, string projectCode)
    {
        var project = DataManager.FindProjectByCode(projectCode, x => x
            .Include(y => y.Categories));
        var categoryCodes = project?.Categories.Select(y => new SelectListItem(y.Code, y.Code)).ToList();
        editingModel.CategorySelectList = categoryCodes ?? new List<SelectListItem>();
    }

    public IActionResult Edit(int id)
    {
        if (DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, RequestedDate).Frozen)
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
        var reportEntry = DataManager.FindReportEntryById(id, q => q
            .Include(qn => qn.Category));
        if (reportEntry == null)
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
        var model = Mapper.Map<ReportEntryForEditingModel>(reportEntry);
        FillSelectListsInEditingModel(model, reportEntry.ProjectCode);
        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(int id, ReportEntryModel reportEntry)
    {
        if (!ModelState.IsValid)
        {
            var editingModel = Mapper.Map<ReportEntryForEditingModel>(reportEntry);
            FillSelectListsInEditingModel(editingModel, reportEntry.Code);
            return View(editingModel);
        }
        if (DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, RequestedDate).Frozen)
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
        var updatedReportEntry = DataManager.FindReportEntryById(id);
        if (updatedReportEntry == null)
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
        var category = DataManager.FindCategoryByProjectCodeAndCode(reportEntry.Code, reportEntry.Subcode);
        updatedReportEntry.CategoryCode = category?.Code;
        updatedReportEntry.Time = reportEntry.Time;
        updatedReportEntry.Description = reportEntry.Description ?? "";
        updatedReportEntry.Timestamp = reportEntry.Timestamp;
        try
        {
            DataManager.UpdateReportEntry(updatedReportEntry);
        }
        catch (DbUpdateConcurrencyException)
        {
            var editingModel = Mapper.Map<ReportEntryForEditingModel>(reportEntry);
            FillSelectListsInEditingModel(editingModel, reportEntry.Code);
            editingModel.Timestamp = DataManager.GetTimestampForReportEntryById(id);
            return View(editingModel);
        }
        return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
    }

    private void FillSelectListsInAddingModel(ReportEntryForAddingModel addingModel)
    {
        var availableProjects = DataManager.GetAllProjects(x => x
            .Include(y => y.Categories)
            .Where(y => y.Active));
        var projectCodes = availableProjects.Select(x => new SelectListItem( $"{x.Name} ({x.Code})", x.Code)).ToList();
        var categoryCodes = availableProjects.ToDictionary(x => x.Code,
            x => x.Categories.Select(y => new SelectListItem(y.Code, y.Code)).ToList());
        addingModel.ProjectSelectList = projectCodes;
        addingModel.ProjectCategorySelectList = categoryCodes;
    }

    public IActionResult Add()
    {
        if (DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, RequestedDate).Frozen)
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
        var model = new ReportEntryForAddingModel { Date = RequestedDate };
        FillSelectListsInAddingModel(model);
        return View(model);
    }

    [HttpPost]
    public IActionResult Add(ReportEntryModel reportEntry)
    {
        if (!ModelState.IsValid)
            goto InvalidModelState;
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, reportEntry.Date);
        if (report.Frozen)
            return RedirectToActionWithError("Index", "Home", new { Date = reportEntry.Date.ToDateString() }, ErrorMessages.GetReportFrozenMessage(reportEntry.Date.ToMonthString()));
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
        mappedReport.ProjectCode = project.Code;
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
        return RedirectToAction("Index", "Home", new { Date = reportEntry.Date.ToDateString() });

        InvalidModelState:
        var addingModel = Mapper.Map<ReportEntryForAddingModel>(reportEntry);
        FillSelectListsInAddingModel(addingModel);
        return View(addingModel);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        if (DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, RequestedDate).Frozen)
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
        try
        {
            DataManager.DeleteReportEntryById(id);
        }
        catch (NotFoundException)
        {
            return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
        }
        return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
