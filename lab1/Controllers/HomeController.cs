using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.DataManager;
using TRS.Models;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDataManager _dataManager;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IDataManager dataManager, IMapper mapper)
        {
            _logger = logger;
            _dataManager = dataManager;
            _mapper = mapper;
        }

        public IActionResult Index(DateTime? date)
        {
            if (Request.Cookies.TryGetValue("user", out var encodedUser))
            {
                var user = JsonSerializer.Deserialize<UserModel>(encodedUser);
                var dateFilter = date ?? DateTime.Today;
                var report = _dataManager.GetReportForUserInMonth(user.Name, dateFilter);
                var filteredEntries = report.Entries
                    .Where(x => x.Date.ToString("yyyy-MM-dd") == dateFilter.ToString("yyyy-MM-dd"));
                var dailyReport = new DailyReportModel
                {
                    Date = dateFilter,
                    Entries = _mapper.Map<List<DailyReportEntryModel>>(filteredEntries)
                };
                return View(dailyReport);
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
