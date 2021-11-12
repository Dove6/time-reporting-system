using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using TRS.Controllers.Attributes;
using TRS.Controllers.Constants;
using TRS.DataManager;
using TRS.DataManager.Exceptions;
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
            if (reportEntry == null)
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
            return View(Mapper.Map<ReportEntryModel>(reportEntry));
        }

        private void FillSelectListsInEditingModel(ReportEntryForEditingModel editingModel, string projectCode)
        {
            var project = DataManager.FindProjectByCode(projectCode);
            var categoryCodes = project.Subactivities.Select(y => new SelectListItem(y.Code, y.Code)).ToList();
            editingModel.CategorySelectList = categoryCodes;
        }

        public IActionResult Edit(int id)
        {
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
            var reportEntry = DataManager.FindReportEntryByDayAndIndex(LoggedInUser.Name, RequestedDate, id);
            if (reportEntry == null)
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
            var model = Mapper.Map<ReportEntryForEditingModel>(reportEntry);
            FillSelectListsInEditingModel(model, reportEntry.Code);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, ReportEntryModel reportEntry)
        {
            if (!ModelState.IsValid)
            {
                var editingModel = Mapper.Map<ReportEntryForEditingModel>(reportEntry);
                FillSelectListsInEditingModel(editingModel, reportEntry.Code);
                return View(editingModel);
            }
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
            var updatedReportEntry = DataManager.FindReportEntryByDayAndIndex(LoggedInUser.Name, RequestedDate, id);
            if (updatedReportEntry == null)
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
            updatedReportEntry.Subcode = reportEntry.Subcode;
            updatedReportEntry.Time = reportEntry.Time;
            updatedReportEntry.Description = reportEntry.Description;
            DataManager.UpdateReportEntry(LoggedInUser.Name, RequestedDate, id, updatedReportEntry);
            return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
        }

        private void FillSelectListsInAddingModel(ReportEntryForAddingModel addingModel)
        {
            var availableProjects = DataManager.GetAllProjects().Where(x => x.Active).ToHashSet();
            var projectCodes = availableProjects.Select(x => new SelectListItem( $"{x.Name} ({x.Code})", x.Code)).ToList();
            var categoryCodes = availableProjects.ToDictionary(x => x.Code,
                x => x.Subactivities.Select(y => new SelectListItem(y.Code, y.Code)).ToList());
            addingModel.ProjectSelectList = projectCodes;
            addingModel.ProjectCategorySelectList = categoryCodes;
        }

        public IActionResult Add()
        {
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
            var model = new ReportEntryForAddingModel { Date = RequestedDate };
            FillSelectListsInAddingModel(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(ReportEntryModel reportEntry)
        {
            if (!ModelState.IsValid)
                goto InvalidModelState;
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, reportEntry.Date).Frozen)
                return RedirectToActionWithError("Index", "Home", new { Date = reportEntry.Date.ToDateString() }, ErrorMessages.GetReportFrozenMessage(reportEntry.Date.ToMonthString()));
            var project = DataManager.FindProjectByCode(reportEntry.Code);
            switch (project)
            {
                case null:
                    ModelState.AddModelError(nameof(reportEntry.Code), ErrorMessages.GetProjectNotFoundMessage(reportEntry.Code));
                    break;
                case { Active: false }:
                    ModelState.AddModelError(nameof(reportEntry.Code), ErrorMessages.GetProjectNoLongerActiveMessage(reportEntry.Code));
                    break;
            }
            if (!ModelState.IsValid)
                goto InvalidModelState;
            DataManager.AddReportEntry(LoggedInUser.Name, Mapper.Map<ReportEntry>(reportEntry));
            return RedirectToAction("Index", "Home", new { Date = reportEntry.Date.ToDateString() });

        InvalidModelState:
            var addingModel = Mapper.Map<ReportEntryForAddingModel>(reportEntry);
            FillSelectListsInAddingModel(addingModel);
            return View(addingModel);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString()));
            try
            {
                DataManager.DeleteReportEntry(LoggedInUser.Name, RequestedDate, id);
            }
            catch (NotFoundException)
            {
                return RedirectToActionWithError("Index", "Home", new { Date = RequestedDate.ToDateString() }, ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id));
            }
            return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
