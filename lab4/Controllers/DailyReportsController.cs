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
public class DailyReportsController : BaseController
{
    private ILogger<DailyReportsController> _logger;

    public DailyReportsController(IDataManager dataManager, IMapper mapper, ILogger<DailyReportsController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("{dateString?}")]
    public IActionResult Index(string? dateString)
    {
        var date = DateTime.Today;
        if (!string.IsNullOrEmpty(dateString)
            && !DateTime.TryParseExact(dateString,
                DateTimeExtensions.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out date))
            return NotFound();
        var report = new Report();
        // TODO: var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, date, x => x
        //     .Include(y => y.Entries)!
        //         .ThenInclude(y => y.Project)
        //     .Include(y => y.Entries)!
        //         .ThenInclude(y => y.Category));
        // TODO: var reportEntries = report.Entries.Where(x => x.Date == date).ToList();
        var reportEntries = new List<ReportEntry>();
        return Ok(new DailyReportModel
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
}
