﻿using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.Extensions;
using Trs.Models.DomainModels;
using Trs.Models.RestModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
[Route("[controller]")]
public class ReportsController : BaseController
{
    private ILogger<ReportsController> _logger;

    public ReportsController(IDataManager dataManager, IMapper mapper, ILogger<ReportsController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    private bool IsMonthString(string monthString)
    {
        return DateTime.TryParseExact(monthString, DateTimeExtensions.MonthFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out _);
    }

    private bool IsDateString(string dateString)
    {
        return DateTime.TryParseExact(dateString, DateTimeExtensions.DateFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out _);
    }

    [HttpGet]
    [Route("{monthString:regex(^\\d{{4}}-\\d{{2}}$)}")]
    public IActionResult Get(string monthString)
    {
        if (!IsMonthString(monthString))
            return BadRequest();  // yyyy-MM
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, monthString, q => q
            .Include(r => r.Entries)
            .Include(r => r.AcceptedTime));
        var summaryEntries = report.Entries!.GroupBy(x => x.ProjectCode)
            .Select(x => new ProjectTimeSummaryEntry
            {
                ProjectCode = x.Key,
                Time = x.Sum(e => e.Time),
                AcceptedTime = report.AcceptedTime!.FirstOrDefault(t => t.ProjectCode == x.Key)?.Time
            }).ToList();
        var response = new ReportDetailsResponse
        {
            Frozen = report.Frozen,
            ProjectTimeSummaries = summaryEntries
        };
        return Ok(response);
    }

    [HttpGet]
    [Route("{dateString:regex(^\\d{{4}}-\\d{{2}}-\\d{{2}}$)}")]
    public IActionResult GetDailyView(string dateString)
    {
        if (!IsDateString(dateString))
            return BadRequest();  // yyyy-MM-dd
        var dayOfMonth = dateString[8..];
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, dateString, x => x
            .Include(r => r.Entries!.Where(e => e.DayOfMonth == dayOfMonth))
                .ThenInclude(e => e.Project)
            .Include(r => r.Entries!.Where(e => e.DayOfMonth == dayOfMonth))
                .ThenInclude(e => e.Category));
        var summaryEntries = report.Entries!.GroupBy(x => x.ProjectCode)
            .Select(x => new ProjectTimeSummaryEntry
            {
                ProjectCode = x.Key,
                Time = x.Sum(e => e.Time)
            }).ToList();
        return Ok(new DailyReportDetailsResponse
        {
            Frozen = report.Frozen,
            Entries = Mapper.Map<List<ReportEntryResponse>>(report.Entries),
            ProjectTimeSummaries = summaryEntries
        });
    }

    [HttpPost]
    [Route("{monthString:regex(^\\d{{4}}-\\d{{2}}$)}/freeze")]
    public IActionResult Freeze(string monthString)
    {
        if (!IsMonthString(monthString))
            return BadRequest();  // yyyy-MM
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, monthString);
        if (!report.Frozen)
            DataManager.FreezeReportByOwnerIdAndMonth(report.OwnerId, report.Month);
        return Ok();
    }

    [HttpPost]
    [Route("{dateString:regex(^\\d{{4}}-\\d{{2}}-\\d{{2}}$)}/entries")]
    public IActionResult PostEntry(string dateString, [FromBody] ReportEntryCreationRequest creationRequest)
    {
        if (!IsDateString(dateString))
            return BadRequest();  // yyyy-MM-dd
        var dayOfMonth = dateString[8..];
        var monthString = dateString[..7];
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, monthString);
        if (report.Frozen)
            return Forbid();
        var category = DataManager.FindCategoryByProjectCodeAndCode(creationRequest.ProjectCode, creationRequest.CategoryCode, q => q
            .Include(c => c.Project));
        if (category == null)
            return BadRequest(ErrorMessages.GetCategoryNotFoundMessage(creationRequest.ProjectCode, creationRequest.CategoryCode));
        if (!category.Project!.Active)
            return Forbid(ErrorMessages.GetProjectNoLongerActiveMessage(creationRequest.ProjectCode));
        var createdReportEntry = Mapper.Map<ReportEntry>(creationRequest);
        createdReportEntry.OwnerId = LoggedInUser!.Id;
        createdReportEntry.ReportMonth = monthString;
        createdReportEntry.DayOfMonth = dayOfMonth;
        DataManager.AddReportEntry(createdReportEntry);
        return CreatedAtAction(nameof(Get), nameof(ReportEntriesController), new { id = createdReportEntry.Id });
    }

    [HttpGet]
    [Route("{dateString}/{path?}")]
    public IActionResult GetBadDateFormat(string dateString, string? path)
    {
        return path switch
        {
            null => BadRequest("yyyy-MM-dd or yyyy-MM"),
            "entries" => BadRequest("yyyy-MM-dd"),
            _ => BadRequest("yyyy-MM")
        };
    }
}
