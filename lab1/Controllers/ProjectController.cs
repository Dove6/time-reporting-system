using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.Controllers.Attributes;
using TRS.Controllers.Constants;
using TRS.DataManager;
using TRS.Extensions;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.Controllers
{
    [ForLoggedInOnly]
    public class ProjectController : BaseController
    {
        private ILogger<ProjectController> _logger;

        public ProjectController(IDataManager dataManager, IMapper mapper, ILogger<ProjectController> logger)
            : base(dataManager, mapper)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var userProjectList = DataManager.FindProjectsByManager(LoggedInUser.Name);
            var projectListModel = new ProjectListModel
            {
                Projects = userProjectList.Select(x =>
                {
                    var mappedProject = Mapper.Map<ProjectModel>(x);
                    var totalAcceptedTime = DataManager.FindReportsByProject(x.Code)
                        .SelectMany(y => y.Accepted)
                        .Where(y => y.Code == x.Code)
                        .Sum(y => y.Time);
                    mappedProject.BudgetLeft = x.Budget - totalAcceptedTime;
                    return mappedProject;
                }).ToList()
            };
            return View(projectListModel);
        }

        public IActionResult Show(string id)
        {
            var project = DataManager.FindProjectByCode(id);
            if (project == null)
                return RedirectToActionWithError("Index", ErrorMessages.GetProjectNotFoundMessage(id));
            var projectWithUsersModel = Mapper.Map<ProjectWithUserSummaryModel>(project);
            var reports = DataManager.FindReportsByProject(id);
            var userSummaries = reports.Where(x => x.Frozen).Select(x =>
            {
                var acceptedSummary = x.Accepted.FirstOrDefault(y => y.Code == project.Code);
                return new ProjectWithUserSummaryEntry
                {
                    Username = x.Owner,
                    Month = x.Month,
                    DeclaredTime = x.Entries.Where(y => y.Code == project.Code).Sum(y => y.Time),
                    AcceptedTime = acceptedSummary?.Time
                };
            }).ToList();
            projectWithUsersModel.BudgetLeft = projectWithUsersModel.Budget - userSummaries.Sum(x => x.AcceptedTime ?? 0);
            projectWithUsersModel.UserSummaries = userSummaries;
            return View(projectWithUsersModel);
        }

        public IActionResult Edit(string id)
        {
            var project = DataManager.FindProjectByCode(id);
            if (project == null)
                return RedirectToActionWithError("Index", ErrorMessages.GetProjectNotFoundMessage(id));
            var projectModel = Mapper.Map<ProjectModel>(project);
            return View(projectModel);
        }

        [HttpPost]
        public IActionResult Edit(ProjectModel projectModel)
        {
            if (!ModelState.IsValid)
                return View(projectModel);
            var inputProject = Mapper.Map<Project>(projectModel);
            var modifiedProject = DataManager.FindProjectByCode(projectModel.Code);
            if (modifiedProject == null)
                return RedirectToActionWithError("Index", ErrorMessages.GetProjectNotFoundMessage(projectModel.Code));
            modifiedProject.Budget = inputProject.Budget;
            modifiedProject.Subactivities = inputProject.Subactivities;
            DataManager.UpdateProject(modifiedProject);
            return RedirectToAction("Index");
        }

        public IActionResult Add()
        {
            return View(new ProjectModel());
        }

        [HttpPost]
        public IActionResult Add(ProjectModel projectModel)
        {
            if (!ModelState.IsValid)
                return View(projectModel);
            var duplicatedProject = DataManager.FindProjectByCode(projectModel.Code);
            if (duplicatedProject != null)
                ModelState.AddModelError(nameof(projectModel.Code), ErrorMessages.GetProjectAlreadyExistingMessage(projectModel.Code));
            if (!ModelState.IsValid)
                return View(projectModel);
            projectModel.Manager = LoggedInUser.Name;
            projectModel.Active = true;
            var project = Mapper.Map<Project>(projectModel);
            DataManager.AddProject(project);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Close(string id)
        {
            var project = DataManager.FindProjectByCode(id);
            if (project == null)
                return RedirectToActionWithError("Index", ErrorMessages.GetProjectNotFoundMessage(id));
            project.Active = false;
            DataManager.UpdateProject(project);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateAcceptedTime(string id, string username, int? acceptedTime)
        {
            var project = DataManager.FindProjectByCode(id);
            if (project == null)
                return RedirectToActionWithError("Index", ErrorMessages.GetProjectNotFoundMessage(id));
            switch (acceptedTime)
            {
                case null:
                    return RedirectToAction("Show", new { Id = id });
                case < 0:
                    return RedirectToActionWithError("Show", new { Id = id }, ErrorMessages.AcceptedTimeNegative);
            }
            var report = DataManager.FindReportByUserAndMonth(username, RequestedDate);
            if (project.Manager != LoggedInUser.Name || report.Entries.All(x => x.Code != id))
                return RedirectToActionWithError("Show", new { Id = id }, ErrorMessages.GetNoAccessToAcceptedTimeMessage(username, RequestedDate.ToMonthString()));
            var accepted = new AcceptedTime { Code = id, Time = acceptedTime.Value };
            DataManager.SetAcceptedTime(username, RequestedDate, accepted);
            return RedirectToAction("Show", new { Id = id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
