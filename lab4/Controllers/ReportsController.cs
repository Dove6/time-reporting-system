using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Extensions;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

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

    [HttpGet]
    [Route("{monthString?}")]
    public IActionResult Index(string? monthString)
    {
        var month = DateTime.Today.TrimToMonth();
        if (!string.IsNullOrEmpty(monthString)
            && !DateTime.TryParseExact(monthString,
                DateTimeExtensions.MonthFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out month))
            return NotFound();
        // TODO: var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, month, x => x
        //     .Include(y => y.Entries)
        //     .Include(y => y.AcceptedTime));
        var report = new Report();
        var summaryEntries = report.Entries.GroupBy(x => x.ProjectCode)
            .Select(x => new ProjectTimeSummaryEntry
            {
                ProjectCode = x.Key,
                Time = x.Sum(y => y.Time),
                AcceptedTime = report.AcceptedTime.FirstOrDefault(y => y.ProjectCode == x.Key)?.Time
            }).ToList();
        var model = new MonthlySummaryModel
        {
            Month = month,
            Frozen = report.Frozen,
            ProjectTimeSummaries = summaryEntries,
            TotalTime = summaryEntries.Sum(x => x.Time),
            TotalAcceptedTime = summaryEntries.Sum(x => x.AcceptedTime ?? 0)
        };
        return Ok(model);
    }

    [HttpPost]
    [Route("{monthString?}/freeze")]
    public IActionResult Freeze(string? monthString)
    {
        var month = DateTime.Today.TrimToMonth();
        if (!string.IsNullOrEmpty(monthString)
            && !DateTime.TryParseExact(monthString,
                DateTimeExtensions.MonthFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out month))
            return NotFound();
        // TODO: var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, month);
        var report = new Report();
        // TODO: DataManager.FreezeReportById(report.Id);
        return Ok();
    }

    [HttpPatch]
    [Route("{monthString?}/accepted")]
    public IActionResult UpdateAcceptedTime(string? monthString, [FromBody] string code, [FromBody] string username, [FromBody] [Range(0, int.MaxValue)] int acceptedTime, [FromBody] byte[] timestamp)
    {
        var month = DateTime.Today.TrimToMonth();
        if (!string.IsNullOrEmpty(monthString)
            && !DateTime.TryParseExact(monthString,
                DateTimeExtensions.MonthFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out month))
            return NotFound();
        var project = DataManager.FindProjectByCode(code);
        if (project == null)
            return NotFound();
        // TODO: var report = DataManager.FindOrCreateReportByUsernameAndMonth(username, month, q => q
        //     .Include(qn => qn.Entries));
        var report = new Report();
        if (project.ManagerId != LoggedInUser!.Id || report.Entries.All(x => x.ProjectCode != code))
            return Forbid();
        // TODO: var accepted = new AcceptedTime { ProjectCode = code, Time = acceptedTime, ReportId = report.Id, Timestamp = timestamp };
        var accepted = new AcceptedTime();
        try
        {
            DataManager.SetAcceptedTime(accepted);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(accepted);
        }
        return Ok();
    }
}
