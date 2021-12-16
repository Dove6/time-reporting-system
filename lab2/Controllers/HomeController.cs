using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

public class HomeController : BaseController
{
    private ILogger<HomeController> _logger;

    public HomeController(IDataManager dataManager, IMapper mapper, ILogger<HomeController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [ForLoggedInOnly]
    public IActionResult Index()
    {
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, RequestedDate);
        var reportEntries = report.ReportEntries.Where(x => x.Date == RequestedDate).ToList();
        return View(new DailyReportModel
        {
            Frozen = report.Frozen,
            Entries = Mapper.Map<List<ReportEntryModel>>(reportEntries),
            ProjectTimeSummaries = reportEntries.GroupBy(x => x.ProjectCode)
                .Select(x => new ProjectTimeSummaryEntry
                {
                    ProjectCode = x.Key,
                    Time = x.Sum(y => y.Time)
                }).ToList(),
            TotalTime = reportEntries.Sum(x => x.Time)
        });
    }

    [ForNotLoggedInOnly]
    public IActionResult NotLoggedIn() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
