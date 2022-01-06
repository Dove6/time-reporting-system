using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
[Route("[controller]")]
public class ProjectsController : BaseController
{
    private ILogger<ProjectsController> _logger;

    public ProjectsController(IDataManager dataManager, IMapper mapper, ILogger<ProjectsController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    [HttpGet]
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
        return Ok(projectListModel);
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult Show(string id)
    {
        var project = DataManager.FindProjectByCode(id, q => q
            .Include(qn => qn.Categories));
        if (project == null)
            return NotFound();
        var projectWithUsersModel = Mapper.Map<ProjectWithUserSummaryModel>(project);
        var reports = DataManager.FindReportsByProject(id, x => x
            .Include(y => y.AcceptedTime)
            .Include(y => y.Owner)
            .Include(y => y.Entries));
        var userSummaries = reports.Where(x => x.Frozen).Select(x =>
        {
            var acceptedSummary = x.AcceptedTime.FirstOrDefault(y => y.ProjectCode == project.Code);
            return new ProjectWithUserSummaryEntry
            {
                Username = x.Owner.Name,
                // TODO: Month = x.Month,
                Month = DateTime.Today,
                DeclaredTime = x.Entries.Where(y => y.ProjectCode == project.Code).Sum(y => y.Time),
                AcceptedTime = acceptedSummary?.Time,
                Timestamp = acceptedSummary?.Timestamp
            };
        }).ToList();
        projectWithUsersModel.BudgetLeft = projectWithUsersModel.Budget - userSummaries.Sum(x => x.AcceptedTime ?? 0);
        projectWithUsersModel.UserSummaries = userSummaries;
        return Ok(projectWithUsersModel);
    }

    [HttpPatch]
    [Route("{id}")]
    public IActionResult Edit(string id, [FromBody] ProjectModel projectModel)
    {
        var inputProject = Mapper.Map<Project>(projectModel);
        var modifiedProject = DataManager.FindProjectByCode(projectModel.Code, q => q
            .Include(qn => qn.Categories));
        modifiedProject.Timestamp = projectModel.Timestamp;
        if (modifiedProject == null)
        {
            ModelState.AddModelError(nameof(projectModel.Code), ErrorMessages.GetProjectNotFoundMessage(projectModel.Code));
            return BadRequest();
        }
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
            return Conflict(projectModel);
        }
        return Ok();
    }

    [HttpPost]
    public IActionResult Add([FromBody] ProjectModel projectModel)
    {
        var duplicatedProject = DataManager.FindProjectByCode(projectModel.Code);
        if (duplicatedProject != null)
        {
            ModelState.AddModelError(nameof(projectModel.Code), ErrorMessages.GetProjectAlreadyExistingMessage(projectModel.Code));
            return BadRequest();
        }
        projectModel.Active = true;
        var project = Mapper.Map<Project>(projectModel);
        project.ManagerId = LoggedInUser!.Id;
        DataManager.AddProject(project);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/close")]
    public IActionResult Close(string id)
    {
        var project = DataManager.FindProjectByCode(id);
        if (project == null)
            return NotFound();
        project.Active = false;
        DataManager.UpdateProject(project);
        return Ok();
    }
}
