using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.DataManager;
using Trs.Extensions;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
[Route("[controller]")]
public class DailyReportController : BaseController
{
    private ILogger<DailyReportController> _logger;

    public DailyReportController(IDataManager dataManager, IMapper mapper, ILogger<DailyReportController> logger)
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
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(LoggedInUser!.Name, date, x => x
            .Include(y => y.ReportEntries)!
                .ThenInclude(y => y.Project)
            .Include(y => y.ReportEntries)!
                .ThenInclude(y => y.Category));
        var reportEntries = report.ReportEntries.Where(x => x.Date == date).ToList();
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
