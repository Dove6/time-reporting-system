using System;
using System.Diagnostics;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.DataManager;
using TRS.Models;
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
            var user = LoggedInUser;
            if (user == null)
                return View("IndexNotLoggedIn");
            var dateFilter = date ?? DateTime.Today;
            var report = DataManager.FindReportByUserAndMonth(user, dateFilter);
            return View(new DailyReportModel { Date = dateFilter, Report = report });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
