using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TRS.Controllers.Attributes;
using TRS.DataManager;
using TRS.Extensions;
using TRS.Models.ViewModels;

namespace TRS.Controllers;

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
        var report = DataManager.FindReportByUserAndMonth(LoggedInUser!.Name, RequestedDate);
        var summaryEntries = report.Entries.GroupBy(x => x.Code)
            .Select(x => new ProjectTimeSummaryEntry
            {
                ProjectCode = x.Key,
                Time = x.Sum(y => y.Time),
                AcceptedTime = report.Accepted.FirstOrDefault(y => y.Code == x.Key)?.Time
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
        DataManager.FreezeReport(LoggedInUser!.Name, RequestedDate);
        return RedirectToAction("Index", new { Date = RequestedDate.ToDateString() });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
