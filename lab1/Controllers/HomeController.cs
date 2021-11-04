using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.Models;
using TRS.Models.ViewModels;

namespace TRS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(DateTime? date)
        {
            var dailyReport = new DailyReportModel
            {
                Date = date ?? DateTime.Now,
                Entries = new List<DailyReportEntryModel>
                {
                    new() { Code = "Projekt 1", Subcode = "Kategoria 1", Time = 45, Description = "Opis..."},
                    new() { Code = "Projekt 1", Subcode = "Kategoria 2", Time = 15, Description = "Opis..."},
                    new() { Code = "Projekt 2", Subcode = "", Time = 45, Description = "Uwagi..."},
                    new() { Code = "Projekt 2", Subcode = "Kategoria 1", Time = 45, Description = "Opis..."},
                    new() { Code = "Projekt 2", Subcode = "Kategoria 2", Time = 45, Description = "Opis..."},
                    new() { Code = "Projekt 2", Subcode = "Kategoria 3", Time = 45, Description = "Opis..."},
                    new() { Code = "Projekt 3", Subcode = "", Time = 30, Description = "Opis..."}
                }
            };
            return View(dailyReport);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
