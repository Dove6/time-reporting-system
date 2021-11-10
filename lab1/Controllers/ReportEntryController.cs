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
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id);
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
            return View(Mapper.Map<ReportEntryModel>(reportEntry));
        }

        public IActionResult Edit(int id)
        {
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString());
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
            var reportEntry = DataManager.FindReportEntryByDayAndIndex(LoggedInUser.Name, RequestedDate, id);
            if (reportEntry == null)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id);
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
            var project = DataManager.FindProjectByCode(reportEntry.Code);
            if (project == null)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetProjectNotFoundMessage(reportEntry.Code);
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
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
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString());
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
            var reportEntry = DataManager.FindReportEntryByDayAndIndex(LoggedInUser.Name, RequestedDate, id);
            if (reportEntry == null)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id);
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
            reportEntry.Subcode = reportEntryUpdate.Subcode;
            reportEntry.Time = reportEntryUpdate.Time;
            reportEntry.Description = reportEntryUpdate.Description;
            DataManager.UpdateReportEntry(LoggedInUser.Name, RequestedDate, id, reportEntry);
            return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
        }

        public IActionResult Add()
        {
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString());
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
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
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, reportEntry.Date).Frozen)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportFrozenMessage(reportEntry.Date.ToMonthString());
                return RedirectToAction("Index", "Home", new { Date = reportEntry.Date.ToDateString() });
            }
            var project = DataManager.FindProjectByCode(reportEntry.Code);
            if (project == null)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetProjectNotFoundMessage(reportEntry.Code);
                return RedirectToAction("Add", new { Date = reportEntry.Date.ToDateString() });
            }
            if (!project.Active)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetProjectNoLongerActiveMessage(reportEntry.Code);
                return RedirectToAction("Add", new { Date = reportEntry.Date.ToDateString() });
            }
            DataManager.AddReportEntry(LoggedInUser.Name, Mapper.Map<ReportEntry>(reportEntry));
            return RedirectToAction("Index", "Home", new { Date = reportEntry.Date.ToDateString() });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (DataManager.FindReportByUserAndMonth(LoggedInUser.Name, RequestedDate).Frozen)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportFrozenMessage(RequestedDate.ToMonthString());
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
            }
            try
            {
                DataManager.DeleteReportEntry(LoggedInUser.Name, RequestedDate, id);
            }
            catch (NotFoundException)
            {
                TempData[ErrorTempDataKey] = ErrorMessages.GetReportEntryNotFoundMessage(RequestedDate, id);
                return RedirectToAction("Index", "Home", new { Date = RequestedDate.ToDateString() });
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
