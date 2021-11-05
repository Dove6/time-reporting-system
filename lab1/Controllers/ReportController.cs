using System;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.DataManager;
using TRS.Models;

namespace TRS.Controllers
{
    public class ReportController : BaseController
    {
        private ILogger<ReportController> _logger;

        public ReportController(IDataManager dataManager, IMapper mapper, ILogger<ReportController> logger)
            : base(dataManager, mapper)
        {
            _logger = logger;
        }

        public IActionResult Index(DateTime? date)
        {
            var user = LoggedInUser;
            var dateFilter = date ?? DateTime.Today;
            var report = DataManager.FindReportByUserAndMonth(user, dateFilter);
            return View(report);
        }

        [HttpPost]
        public IActionResult Freeze(DateTime? date)
        {
            var user = LoggedInUser;
            var dateFilter = date ?? DateTime.Today;
            var report = DataManager.FindReportByUserAndMonth(user, dateFilter);
            report.Frozen = true;
            DataManager.UpdateReport(report);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
