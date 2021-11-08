using System;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TRS.Controllers.Attributes;
using TRS.DataManager;
using TRS.Models;
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
            var userProjectList = DataManager.FindProjectsByManager(LoggedInUser);
            var projectListModel = new ProjectListModel
            {
                Activities = userProjectList.Select(x =>
                {
                    var mappedProject = Mapper.Map<ProjectModel>(x);
                    var totalAcceptedTime = DataManager.FindReportByProject(x)
                        .SelectMany(y => y.Accepted)
                        .Where(y => y.Code == x.Code)
                        .Sum(y => y.Time);
                    mappedProject.BudgetLeft = x.Budget - totalAcceptedTime;
                    return mappedProject;
                }).ToArray()
            };
            return View(projectListModel);
        }

        public IActionResult Show(string id)
        {
            var project = DataManager.FindProjectByCode(id);
            var projectModel = Mapper.Map<ProjectModel>(project);
            var reports = DataManager.FindReportByProject(project);
            var userSummaries = reports.Where(x => x.Frozen).Select(x =>
            {
                var acceptedSummary = x.Accepted.FirstOrDefault(y => y.Code == project.Code);
                return new UserProjectMonthlySummaryModel
                {
                    Username = x.Owner,
                    Month = x.Month,
                    DeclaredTime = x.Entries.Where(y => y.Code == project.Code).Sum(y => y.Time),
                    AcceptedTime = acceptedSummary?.Time
                };
            }).ToArray();
            projectModel.BudgetLeft = projectModel.Budget - userSummaries.Sum(x => x.AcceptedTime ?? 0);
            var projectWithUsersModel = new ProjectWithUserSummaryModel
            {
                Project = projectModel,
                UserSummaries = userSummaries
            };
            return View(projectWithUsersModel);
        }

        public IActionResult Edit(string id)
        {
            var project = DataManager.FindProjectByCode(id);
            var projectModel = Mapper.Map<ProjectModel>(project);
            return View(projectModel);
        }

        [HttpPost]
        public IActionResult Edit(ProjectModel projectModel)
        {
            var inputProject = Mapper.Map<Project>(projectModel);
            var modifiedProject = DataManager.FindProjectByCode(projectModel.Code);
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
            project.Active = false;
            DataManager.UpdateProject(project);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateAcceptedTime(string id, string username, DateTime month, int? acceptedTime)
        {
            if (!acceptedTime.HasValue)
                return RedirectToAction("Show", new { Id = id });
            var report = DataManager.FindReportByUserAndMonth(new User { Name = username }, month);
            var accepted = new AcceptedTime { Code = id, Time = acceptedTime.Value };
            if (report.Accepted.Contains(accepted))
                DataManager.UpdateAcceptedTime(new User { Name = username }, month, accepted);
            else
                DataManager.AddAcceptedTime(new User { Name = username }, month, accepted);
            return RedirectToAction("Show", new { Id = id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
