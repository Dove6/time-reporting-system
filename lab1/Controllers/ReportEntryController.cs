using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var project = DataManager.FindProjectByCode(report.Entries[id].Code);
            var categoryCodes = new List<SelectListItem> { new("nieokreślony", "") }.Concat(
                project.Subactivities.Select(y => new SelectListItem(y.Code, y.Code))).ToList();
            var model = new ReportEntryForEditingModel
            {
                ReportEntry = report.Entries[id],
                CategorySelectList = categoryCodes
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(DateTime? date, int id, ReportEntryUpdateModel reportEntryUpdate)
        {
            var dateFilter = date ?? DateTime.Today;
            var report = DataManager.FindReportByUserAndMonth(LoggedInUser, dateFilter);
            report.Entries[id].Subcode = reportEntryUpdate.Subcode;
            report.Entries[id].Time = reportEntryUpdate.Time;
            report.Entries[id].Description = reportEntryUpdate.Description;
            DataManager.UpdateReport(report);
            return RedirectToAction("Index", "Home", new { Date = dateFilter.ToString("yyyy-MM-dd") });
        }

        public IActionResult Add(DateTime? date)
        {
            var initialDate = date ?? DateTime.Today;
            var availableProjects = DataManager.GetAllProjects().Where(x => x.Active);
            var projectCodes = availableProjects.Select(x => new SelectListItem(x.Name, x.Code)).ToList();
            var categoryCodes = availableProjects.ToDictionary(x => x.Code,
                x => new List<SelectListItem> { new("nieokreślony", "") }.Concat(
                    x.Subactivities.Select(y => new SelectListItem(y.Code, y.Code)).ToList()
                    ).ToList());
            var model = new ReportEntryForAddingModel
            {
                ReportEntry = new ReportEntry { Date = initialDate },
                ProjectSelectList = projectCodes,
                ProjectCategorySelectList = categoryCodes
            };
            return View(model);
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
