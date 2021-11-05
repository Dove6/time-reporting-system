using System;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.DataManager;
using TRS.Models;
using TRS.Models.DomainModels;
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

        public IActionResult Index(DateTime? date)
        {
            if (HttpContext.Items.TryGetValue("user", out var user))
            {
                var dateFilter = date ?? DateTime.Today;
                var report = DataManager.FindReportByUserAndMonth((User)user, dateFilter);
                return View(new DailyReportModel { Date = dateFilter, Report = report });
            }
            return View("IndexNotLoggedIn");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
