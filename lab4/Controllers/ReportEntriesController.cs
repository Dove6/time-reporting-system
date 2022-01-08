using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.DataManager.Exceptions;
using Trs.Models.RestModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
[Route("api/[controller]/{id:int}")]
public class ReportEntriesController : BaseController
{
    private ILogger<ReportEntriesController> _logger;

    public ReportEntriesController(IDataManager dataManager, IMapper mapper, ILogger<ReportEntriesController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get(int id)
    {
        var reportEntry = DataManager.FindReportEntryById(id, x => x
            .Include(y => y.Category));
        if (reportEntry == null)
            return NotFound();
        return Ok(Mapper.Map<ReportEntryResponse>(reportEntry));
    }

    [HttpPatch]
    public IActionResult Patch(int id, [FromBody] ReportEntryUpdateRequest updateRequest)
    {
        var originalReportEntry = DataManager.FindReportEntryById(id, q => q
            .Include(e => e.Report));
        if (originalReportEntry == null)
            return NotFound();
        if (originalReportEntry.Report!.Frozen)
            return Forbid();
        var category =
            DataManager.FindCategoryByProjectCodeAndCode(originalReportEntry.ProjectCode, updateRequest.CategoryCode);
        if (category == null)
            return BadRequest(ErrorMessages.GetCategoryNotFoundMessage(originalReportEntry.ProjectCode, updateRequest.CategoryCode));
        var updatedReportEntry = Mapper.Map(updateRequest, originalReportEntry);
        try
        {
            DataManager.UpdateReportEntry(updatedReportEntry);
            return Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }
    }

    [HttpDelete]
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
