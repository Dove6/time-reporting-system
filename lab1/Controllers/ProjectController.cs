using System;
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
                Activities = Mapper.Map<ProjectModel[]>(userProjectList)
            };
            return View(projectListModel);
        }

        public IActionResult Show(string id)
        {
            var project = DataManager.FindProjectByCode(id);
            var projectModel = Mapper.Map<ProjectModel>(project);
            var users = DataManager.FindUsersByProject(project).Select(x => x.Name).ToArray();
            var projectWithUsersModel = new ProjectWithUsersModel
            {
                Project = projectModel,
                Users = users
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
