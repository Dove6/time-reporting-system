using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Trs.DataManager.Exceptions;
using Trs.Extensions;
using Trs.Models.DomainModels;

namespace Trs.DataManager;

public class DbDataManager : IDataManager
{
    private readonly TrsDbContext _dbContext;
    private readonly IMapper _mapper;

    public DbDataManager(TrsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public int AddUser(User user)
    {
        var addedUser = _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return addedUser.Entity.Id;
    }

    public User? FindUserByName(string name) =>
        _dbContext.Users
            .AsNoTracking()
            .FirstOrDefault(x => x.Name == name);

    public HashSet<User> GetAllUsers() =>
        _dbContext.Users
            .AsNoTracking()
            .ToHashSet();

    public string AddProject(Project project)
    {
        var addedProject = _dbContext.Projects.Add(project);
        _dbContext.SaveChanges();
        return addedProject.Entity.Code;
    }

    public Project? FindProjectByCode(string code) =>
        _dbContext.Projects
            .AsNoTracking()
            .FirstOrDefault(x => x.Code == code);

    public HashSet<Project> FindProjectsByManager(string managerName)
    {
        var foundManager = _dbContext.Users
            .FirstOrDefault(x => x.Name == managerName);
        if (foundManager == null)
            return new HashSet<Project>();
        return _dbContext.Projects
            .AsNoTracking()
            .Where(x => x.ManagerId == foundManager.Id)
            .ToHashSet();
    }

    public HashSet<Project> GetAllProjects() =>
        _dbContext.Projects
            .AsNoTracking()
            .ToHashSet();

    public void UpdateProject(Project project)
    {
        _dbContext.Projects.Update(project);
        _dbContext.SaveChanges();
    }

    public Report FindOrCreateReportByUsernameAndMonth(string username, DateTime month)
    {
        month = month.TrimToMonth();
        var foundOwner = _dbContext.Users
            .FirstOrDefault(x => x.Name == username);
        if (foundOwner == null)
            throw new NotFoundException();
        var foundReport = _dbContext.Reports
            .AsNoTracking()
            .Include(x => x.ReportEntries)
            .FirstOrDefault(x => x.OwnerId == foundOwner.Id && x.Month == month);
        if (foundReport != null)
            return foundReport;
        var newReport = new Report { OwnerId = foundOwner.Id, Month = month };
        var addedReport = _dbContext.Reports.Add(newReport);
        _dbContext.SaveChanges();
        addedReport.Collection(x => x.ReportEntries!).Load();
        return addedReport.Entity;
    }

    public HashSet<Report> FindReportsByProject(string projectCode)
    {
        var foundProject = _dbContext.Projects
            .AsNoTracking()
            .Include(x => x.ReportEntries)!
            .ThenInclude(x => x.Report)
            .FirstOrDefault(x => x.Code == projectCode);
        if (foundProject == null)
            return new HashSet<Report>();
        return foundProject.ReportEntries!
            .Select(x => x.Report)
            .Distinct()
            .ToHashSet()!;
    }

    public void FreezeReportById(int reportId)
    {
        var foundReport = _dbContext.Reports
            .FirstOrDefault(x => x.Id == reportId);
        if (foundReport == null)
            throw new NotFoundException();
        foundReport.Frozen = true;
        _dbContext.Update(foundReport);
        _dbContext.SaveChanges();
    }

    public int AddReportEntry(ReportEntry reportEntry)
    {
        var addedReportEntry = _dbContext.ReportEntries.Add(reportEntry);
        _dbContext.SaveChanges();
        return addedReportEntry.Entity.Id;
    }

    public void DeleteReportEntryById(int reportEntryId)
    {
        var foundReportEntry = _dbContext.ReportEntries
            .FirstOrDefault(x => x.Id == reportEntryId);
        if (foundReportEntry == null)
            throw new NotFoundException();
        _dbContext.ReportEntries.Remove(foundReportEntry);
        _dbContext.SaveChanges();
    }

    public ReportEntry? FindReportEntryById(int reportEntryId) =>
        _dbContext.ReportEntries
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == reportEntryId);

    public HashSet<ReportEntry> FindReportEntriesByUsernameAndDay(string username, DateTime day) =>
        _dbContext.ReportEntries
            .AsNoTracking()
            .Include(x => x.Report)
            .ThenInclude(x => x!.Owner)
            .Where(x => x.Date == day.Date && x.Report!.Owner!.Name == username)
            .ToHashSet();

    public HashSet<ReportEntry> FindReportEntriesByUsernameAndMonth(string username, DateTime month) =>
        _dbContext.ReportEntries
            .AsNoTracking()
            .Include(x => x.Report)
            .ThenInclude(x => x!.Owner)
            .Where(x => x.Date.TrimToMonth() == month.TrimToMonth() && x.Report!.Owner!.Name == username)
            .ToHashSet();

    public void UpdateReportEntry(ReportEntry reportEntry)
    {
        _dbContext.ReportEntries.Update(reportEntry);
        _dbContext.SaveChanges();
    }

    public void SetAcceptedTime(AcceptedTime acceptedTime)
    {
        var foundAccepted = _dbContext.AcceptedTime
            .FirstOrDefault(x => x.ReportId == acceptedTime.ReportId && x.ProjectCode == acceptedTime.ProjectCode);
        if (foundAccepted != null)
            _dbContext.AcceptedTime.Update(acceptedTime);
        else
            _dbContext.AcceptedTime.Add(acceptedTime);
        _dbContext.SaveChanges();
    }

    public Category? FindCategoryByProjectCodeAndCode(string projectCode, string categoryCode) =>
        _dbContext.Categories
            .FirstOrDefault(x => x.ProjectCode == projectCode && x.Code == categoryCode);
}
