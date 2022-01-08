using System.Globalization;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trs.Controllers.Attributes;
using Trs.Controllers.Constants;
using Trs.DataManager;
using Trs.Extensions;
using Trs.Models.DomainModels;
using Trs.Models.RestModels;

namespace Trs.Controllers;

[ForLoggedInOnly]
public class ProjectsController : BaseController
{
    private ILogger<ProjectsController> _logger;

    public ProjectsController(IDataManager dataManager, IMapper mapper, ILogger<ProjectsController> logger)
        : base(dataManager, mapper)
    {
        _logger = logger;
    }

    private bool IsMonthString(string monthString)
    {
        return DateTime.TryParseExact(monthString, DateTimeExtensions.MonthFormat, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out _);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var projectList = DataManager.GetAllProjects(q => q
            .Include(p => p.Categories));
        return Ok(Mapper.Map<List<ProjectListResponseEntry>>(projectList));
    }

    [HttpGet("managed")]
    public IActionResult GetManaged()
    {
        var managedProjectList = DataManager.FindProjectsByManager(LoggedInUser!.Name, q => q
            .Include(p => p.Categories)
            .Include(p => p.AcceptedTime));
        return Ok(Mapper.Map<List<ManagedProjectListResponseEntry>>(managedProjectList));
    }

    [HttpPut("{projectCode}")]
    public IActionResult Put(string projectCode, [FromBody] ProjectCreationRequest creationRequest)
    {
        var duplicatedProject = DataManager.FindProjectByCode(projectCode);
        if (duplicatedProject != null)
            return Conflict(ErrorMessages.GetProjectAlreadyExistingMessage(projectCode));
        var createdProject = Mapper.Map<Project>(creationRequest);
        createdProject.Code = projectCode;
        createdProject.Active = true;
        createdProject.ManagerId = LoggedInUser!.Id;
        DataManager.AddProject(createdProject);
        Mapper.Map<List<Category>>(creationRequest.Categories.Prepend(new CategoryModel { Code = "" }))
            .ForEach(c =>
            {
                c.ProjectCode = projectCode;
                DataManager.AddCategory(c);
            });
        return CreatedAtAction(nameof(Get), new { projectCode });
    }

    [HttpGet("{projectCode}")]
    public IActionResult Get(string projectCode)
    {
        var project = DataManager.FindProjectByCode(projectCode, q => q
            .Include(p => p.Categories));
        if (project == null)
            return NotFound();
        if (project.ManagerId != LoggedInUser!.Id)
            return Forbid();
        var projectDetails = Mapper.Map<ProjectDetailsResponse>(project);
        var reports = DataManager.FindReportsByProject(projectCode, q => q
            .Include(r => r.AcceptedTime)
            .Include(r => r.Owner));
        var userSummaries = reports.Where(x => x.Frozen).Select(x =>
        {
            var acceptedTime = x.AcceptedTime!.FirstOrDefault(y => y.ProjectCode == project.Code);
            var entries = x.Entries!.Where(y => y.ProjectCode == project.Code);
            return new AcceptedTimeListEntry
            {
                Username = x.Owner!.Name,
                Month = x.Month,
                DeclaredTime = entries.Sum(y => y.Time),
                AcceptedTime = acceptedTime?.Time,
                Timestamp = acceptedTime?.Timestamp
            };
        }).ToList();
        projectDetails.AcceptedTime = userSummaries;
        return Ok(projectDetails);
    }

    [HttpPatch("{projectCode}")]
    public IActionResult Patch(string projectCode, [FromBody] ProjectUpdateRequest updateRequest)
    {
        var originalProject = DataManager.FindProjectByCode(projectCode, q => q
            .Include(p => p.Categories));
        if (originalProject == null)
            return NotFound();
        if (originalProject.ManagerId != LoggedInUser!.Id)
            return Forbid();
        var updatedProject = Mapper.Map(updateRequest, originalProject);
        var addedCategories = updatedProject.Categories!.Select(x => x.Code)
            .Except(originalProject.Categories!.Select(x => x.Code));
        var removedCategories = originalProject.Categories!.Select(x => x.Code)
            .Except(updatedProject.Categories!.Select(x => x.Code));
        if (removedCategories.Any())
            return BadRequest(ErrorMessages.CannotRemoveCategory);
        updatedProject.Categories = null;
        try
        {
            DataManager.UpdateProject(updatedProject);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }
        foreach (var addedCategory in addedCategories)
            DataManager.AddCategory(new Category { ProjectCode = originalProject.Code, Code = addedCategory });
        return Ok();
    }

    [HttpPost("{projectCode}/close")]
    public IActionResult Close(string projectCode)
    {
        var closedProject = DataManager.FindProjectByCode(projectCode);
        if (closedProject == null)
            return NotFound();
        if (closedProject.ManagerId != LoggedInUser!.Id)
            return Forbid();
        if (closedProject.Active == false)
            return Ok();
        closedProject.Active = false;
        DataManager.UpdateProject(closedProject);
        return Ok();
    }

    [HttpPut("{projectCode}/acceptedtime/{username}/{monthString}")]
    public IActionResult PutAcceptedTime(string projectCode, string username, string monthString, [FromBody] AcceptedTimeUpdateRequest updateRequest)
    {
        if (!IsMonthString(monthString))
            return BadRequest();  // yyyy-MM
        var project = DataManager.FindProjectByCode(projectCode);
        if (project == null)
            return NotFound();
        var report = DataManager.FindOrCreateReportByUsernameAndMonth(username, monthString, q => q
            .Include(r => r.Entries));
        if (project.ManagerId != LoggedInUser!.Id || report.Entries!.All(x => x.ProjectCode != projectCode))
            return Forbid();
        var updatedAcceptedTime = Mapper.Map<AcceptedTime>(updateRequest);
        updatedAcceptedTime.ProjectCode = project.Code;
        updatedAcceptedTime.OwnerId = report.OwnerId;
        updatedAcceptedTime.ReportMonth = report.Month;
        try
        {
            DataManager.SetAcceptedTime(updatedAcceptedTime);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }
        return Ok();
    }
}
