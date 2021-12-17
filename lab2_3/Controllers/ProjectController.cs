using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        var userProjectList = DataManager.FindProjectsByManager(LoggedInUser!.Name, q => q
            .Include(qn => qn.Categories));
        var projectListModel = new ProjectListModel
        {
            Projects = userProjectList.Select(x =>
            {
                var mappedProject = Mapper.Map<ProjectModel>(x);
                var totalAcceptedTime = DataManager.FindReportsByProject(x.Code, q => q
                        .Include(y => y.AcceptedTime))
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
        var project = DataManager.FindProjectByCode(id, q => q
            .Include(qn => qn.Categories));
        if (project == null)
            return RedirectToActionWithError("Index", ErrorMessages.GetProjectNotFoundMessage(id));
        var projectWithUsersModel = Mapper.Map<ProjectWithUserSummaryModel>(project);
        var reports = DataManager.FindReportsByProject(id, x => x
            .Include(y => y.AcceptedTime)
            .Include(y => y.Owner)
            .Include(y => y.ReportEntries));
        var userSummaries = reports.Where(x => x.Frozen).Select(x =>
        {
            var acceptedSummary = x.AcceptedTime.FirstOrDefault(y => y.ProjectCode == project.Code);
            return new ProjectWithUserSummaryEntry
            {
                Username = x.Owner.Name,
                Month = x.Month,
                DeclaredTime = x.ReportEntries.Where(y => y.ProjectCode == project.Code).Sum(y => y.Time),
                AcceptedTime = acceptedSummary?.Time,
                Timestamp = acceptedSummary?.Timestamp
            };
        }).ToList();
        projectWithUsersModel.BudgetLeft = projectWithUsersModel.Budget - userSummaries.Sum(x => x.AcceptedTime ?? 0);
        projectWithUsersModel.UserSummaries = userSummaries;
        return View(projectWithUsersModel);
    }

    public IActionResult Edit(string id)
    {
        var project = DataManager.FindProjectByCode(id, q => q
            .Include(qn => qn.Manager)
            .Include(qn => qn.Categories));
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
        var modifiedProject = DataManager.FindProjectByCode(projectModel.Code, q => q
            .Include(qn => qn.Categories));
        modifiedProject.Timestamp = projectModel.Timestamp;
        if (modifiedProject == null)
            return RedirectToActionWithError("Index", ErrorMessages.GetProjectNotFoundMessage(projectModel.Code));
        modifiedProject.Budget = inputProject.Budget;
        var addedCategories = inputProject.Categories.Select(x => x.Code)
            .Except(modifiedProject.Categories.Select(x => x.Code));
        foreach (var addedCategory in addedCategories)
            DataManager.AddCategory(new Category { ProjectCode = modifiedProject.Code, Code = addedCategory });
        modifiedProject.Categories = null;
        try
        {
            DataManager.UpdateProject(modifiedProject);
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.Clear();
            projectModel.Timestamp = DataManager.FindProjectByCode(projectModel.Code)!.Timestamp;
            ModelState.AddModelError(nameof(projectModel.Timestamp), ErrorMessages.ConcurrencyError);
            return View(projectModel);
        }
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
        projectModel.Active = true;
        var project = Mapper.Map<Project>(projectModel);
        project.ManagerId = LoggedInUser!.Id;
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
    public IActionResult UpdateAcceptedTime(string id, string username, int? acceptedTime, byte[] timestamp)
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
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(username, RequestedDate, q => q
            .Include(qn => qn.ReportEntries));
        if (project.ManagerId != LoggedInUser!.Id || report.ReportEntries.All(x => x.ProjectCode != id))
            return RedirectToActionWithError("Show", new { Id = id }, ErrorMessages.GetNoAccessToAcceptedTimeMessage(username, RequestedDate.ToMonthString()));
        var accepted = new AcceptedTime { ProjectCode = id, Time = acceptedTime.Value, ReportId = report.Id, Timestamp = timestamp };
        try
        {
            DataManager.SetAcceptedTime(accepted);
        }
        catch (DbUpdateConcurrencyException)
        {
            return RedirectToActionWithError("Show", new { Id = id }, ErrorMessages.ConcurrencyError);
        }
        return RedirectToAction("Show", new { Id = id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
