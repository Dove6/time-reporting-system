using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.Extensions;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

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
        var userProjectList = DataManager.FindProjectsByManager(LoggedInUser!.Name);
        var projectListModel = new ProjectListModel
        {
            Projects = userProjectList.Select(x =>
            {
                var mappedProject = Mapper.Map<ProjectModel>(x);
                var totalAcceptedTime = DataManager.FindReportsByProject(x.Code)
                    .SelectMany(y => y.AcceptedTime)
                    .Where(y => y.ProjectCode == x.Code)
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
            var acceptedSummary = x.AcceptedTime.FirstOrDefault(y => y.ProjectCode == project.Code);
            return new ProjectWithUserSummaryEntry
            {
                Username = x.Owner.Name,
                Month = x.Month,
                DeclaredTime = x.ReportEntries.Where(y => y.ProjectCode == project.Code).Sum(y => y.Time),
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
        modifiedProject.Categories = inputProject.Categories;
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
        projectModel.Manager = LoggedInUser!.Name;
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
        if (project.ManagerId != LoggedInUser!.Id || report.ReportEntries.All(x => x.ProjectCode != id))
            return RedirectToActionWithError("Show", new { Id = id }, ErrorMessages.GetNoAccessToAcceptedTimeMessage(username, RequestedDate.ToMonthString()));
        var accepted = new AcceptedTime { ProjectCode = id, Time = acceptedTime.Value };
        DataManager.SetAcceptedTime(username, RequestedDate, accepted);
        return RedirectToAction("Show", new { Id = id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
