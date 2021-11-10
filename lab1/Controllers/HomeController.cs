using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.Controllers.Attributes;
using TRS.DataManager;
using TRS.Models.ViewModels;

namespace TRS.Controllers
{
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
            var report = DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate);
            var reportEntries = report.Entries.Where(x => x.Date == RequestedDate).ToList();
            return View(new DailyReportModel
            {
                Frozen = report.Frozen,
                Entries = Mapper.Map<List<DailyReportEntry>>(reportEntries),
                ProjectTimeSummary = reportEntries.GroupBy(x => x.Code)
                    .ToDictionary(x => x.Key, x => x.Sum(y => y.Time)),
                TotalDailyTime = reportEntries.Sum(x => x.Time)
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
}
