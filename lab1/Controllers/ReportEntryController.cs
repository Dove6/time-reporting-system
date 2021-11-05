﻿using System;
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
    public class ReportEntryController : BaseController
    {
        private ILogger<ReportEntryController> _logger;

        public ReportEntryController(IDataManager dataManager, IMapper mapper, ILogger<ReportEntryController> logger)
            : base(dataManager, mapper)
        {
            _logger = logger;
        }

        public IActionResult Show(DateTime? date, int id)
        {
            var user = (User)HttpContext.Items["user"];
            var dateFilter = date ?? DateTime.Today;
            var report = DataManager.FindReportByUserAndMonth(user, dateFilter);
            return View(new DailyReportModel { Date = dateFilter, Report = report });
        }

        public IActionResult Edit(DateTime? date, int id)
        {
            var user = LoggedInUser;
            var dateFilter = date ?? DateTime.Today;
            var report = DataManager.FindReportByUserAndMonth(user, dateFilter);
            return View(new DailyReportModel { Date = dateFilter, Report = report });
        }

        [HttpPost]
        public IActionResult Edit(int id, ReportEntry reportEntry)
        {
            reportEntry.Owner = LoggedInUser;
            var report = DataManager.FindReportByUserAndMonth(LoggedInUser, reportEntry.Date);
            report.Entries[id] = reportEntry;
            DataManager.UpdateReport(report);
            return RedirectToAction("Index", "Home", new { Date = reportEntry.Date.ToString("yyyy-MM-dd") });
        }

        public IActionResult Add(DateTime? date)
        {
            var initialDate = date ?? DateTime.Today;
            var reportEntry = new ReportEntry { Date = initialDate };
            return View(reportEntry);
        }

        [HttpPost]
        public IActionResult Add(ReportEntry reportEntry)
        {
            reportEntry.Owner = LoggedInUser;
            var report = DataManager.FindReportByUserAndMonth(LoggedInUser, reportEntry.Date);
            report.Entries.Add(reportEntry);
            DataManager.UpdateReport(report);
            return RedirectToAction("Index", "Home", new { Date = reportEntry.Date.ToString("yyyy-MM-dd") });
        }

        [HttpPost]
        public IActionResult Delete(DateTime? date, int id)
        {
            var dateFilter = date ?? DateTime.Today;
            var report = DataManager.FindReportByUserAndMonth(LoggedInUser, dateFilter);
            report.Entries.RemoveAt(id);
            DataManager.UpdateReport(report);
            return RedirectToAction("Index", "Home", new { Date = dateFilter.ToString("yyyy-MM-dd") });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}