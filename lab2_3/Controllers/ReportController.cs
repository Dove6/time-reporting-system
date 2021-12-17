using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Extensions;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
public class ReportController : BaseController
{
    private ILogger<ReportController> _logger;

    public ReportController(IDataManager dataManager, IMapper mapper, ILogger<ReportController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, RequestedDate, x => x
            .Include(y => y.ReportEntries)
            .Include(y => y.AcceptedTime));
        var summaryEntries = report.ReportEntries.GroupBy(x => x.ProjectCode)
            .Select(x => new ProjectTimeSummaryEntry
            {
                ProjectCode = x.Key,
                Time = x.Sum(y => y.Time),
                AcceptedTime = report.AcceptedTime.FirstOrDefault(y => y.ProjectCode == x.Key)?.Time
            }).ToList();
        var model = new MonthlySummaryModel
        {
            Month = RequestedDate,
            Frozen = report.Frozen,
            ProjectTimeSummaries = summaryEntries,
            TotalTime = summaryEntries.Sum(x => x.Time),
            TotalAcceptedTime = summaryEntries.Sum(x => x.AcceptedTime ?? 0)
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Freeze()
    {
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, RequestedDate);
        DataManager.FreezeReportById(report.Id);
        return RedirectToAction("Index", new { Date = RequestedDate.ToDateString() });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
