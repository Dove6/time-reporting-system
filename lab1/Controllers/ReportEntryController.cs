using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TRS.Controllers.Attributes;
using TRS.DataManager;
using TRS.Extensions;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.Controllers
{
    [ForLoggedInOnly]
    public class ReportEntryController : BaseController
    {
        private ILogger<ReportEntryController> _logger;

        public ReportEntryController(IDataManager dataManager, IMapper mapper, ILogger<ReportEntryController> logger)
            : base(dataManager, mapper)
        {
            _logger = logger;
        }

        public IActionResult Show(int id)
        {
            var reportEntry = DataManager.FindReportEntryByDayAndIndex(LoggedInUser.Name, RequestedDate, id);
            return View(Mapper.Map<ReportEntryModel>(reportEntry));
        }

        public IActionResult Edit(int id)
        {
            var reportEntry = DataManager.FindReportEntryByDayAndIndex(LoggedInUser.Name, RequestedDate, id);
            var project = DataManager.FindProjectByCode(reportEntry.Code);
            var categoryCodes = new List<SelectListItem> { new("nieokreślony", "") }.Concat(
                project.Subactivities.Select(y => new SelectListItem(y.Code, y.Code))).ToList();
            var model = new ReportEntryForEditingModel
            {
                ReportEntry = Mapper.Map<ReportEntryModel>(reportEntry),
                CategorySelectList = categoryCodes
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, ReportEntryUpdateModel reportEntryUpdate)
        {
            var reportEntry = DataManager.FindReportEntryByDayAndIndex(LoggedInUser.Name, RequestedDate, id);
            reportEntry.Subcode = reportEntryUpdate.Subcode;
            reportEntry.Time = reportEntryUpdate.Time;
            reportEntry.Description = reportEntryUpdate.Description;
            DataManager.UpdateReportEntry(LoggedInUser.Name, RequestedDate, id, reportEntry);
            return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
        }

        public IActionResult Add()
        {
            var availableProjects = DataManager.GetAllProjects().Where(x => x.Active).ToHashSet();
            var projectCodes = availableProjects.Select(x => new SelectListItem(x.Name, x.Code)).ToList();
            var categoryCodes = availableProjects.ToDictionary(x => x.Code,
                x => new List<SelectListItem> { new("nieokreślony", "") }.Concat(
                    x.Subactivities.Select(y => new SelectListItem(y.Code, y.Code)).ToList()
                    ).ToList());
            var model = new ReportEntryForAddingModel
            {
                ReportEntry = new ReportEntryModel { Date = RequestedDate },
                ProjectSelectList = projectCodes,
                ProjectCategorySelectList = categoryCodes
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ReportEntryModel reportEntry)
        {
            DataManager.AddReportEntry(LoggedInUser.Name, Mapper.Map<ReportEntry>(reportEntry));
            return RedirectToAction("Index", "Home", new { Date = reportEntry.Date.ToDateString() });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            DataManager.DeleteReportEntry(LoggedInUser.Name, RequestedDate, id);
            return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
