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
        addedUser.State = EntityState.Detached;
        return addedUser.Entity.Id;
    }

    public byte[]? GetTimestampForReportEntryById(int id) =>
        _dbContext.ReportEntries
            .FirstOrDefault(x => x.Id == id)?
            .Timestamp;

    public User? FindUserById(int id, Func<IQueryable<User>, IQueryable<User>>? modifierFunc = null)
    {
        var query = _dbContext.Users.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public User? FindUserByName(string name, Func<IQueryable<User>, IQueryable<User>>? modifierFunc = null)
    {
        var query = _dbContext.Users.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .FirstOrDefault(x => x.Name == name);
    }

    public List<User> GetAllUsers(Func<IQueryable<User>, IQueryable<User>>? modifierFunc = null)
    {
        var query = _dbContext.Users.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .ToList();
    }

    public void AddProject(Project project)
    {
        var addedProject = _dbContext.Projects.Add(project);
        _dbContext.SaveChanges();
        addedProject.State = EntityState.Detached;
    }

    public void DeleteCategory(Category category)
    {
        _dbContext.Categories.Remove(category);
        _dbContext.SaveChanges();
    }

    public Project? FindProjectByCode(string code, Func<IQueryable<Project>, IQueryable<Project>>? modifierFunc = null)
    {
        var query = _dbContext.Projects.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .FirstOrDefault(x => x.Code == code);
    }

    public List<Project> FindProjectsByManager(string managerName, Func<IQueryable<Project>, IQueryable<Project>>? modifierFunc = null)
    {
        var foundManager = _dbContext.Users
            .FirstOrDefault(x => x.Name == managerName);
        if (foundManager == null)
            return new List<Project>();
        var query = _dbContext.Projects.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .Where(x => x.ManagerId == foundManager.Id)
            .ToList();
    }

    public List<Project> GetAllProjects(Func<IQueryable<Project>, IQueryable<Project>>? modifierFunc = null)
    {
        var query = _dbContext.Projects.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .ToList();
    }

    public void UpdateProject(Project project)
    {
        _dbContext.Projects.Update(project);
        _dbContext.SaveChanges();
    }

    public byte[] GetTimestampForProject(Project project) =>
        _dbContext.Entry(project).Entity.Timestamp;

    public Report FindOrCreateReportByUsernameAndMonth(string username, DateTime month, Func<IQueryable<Report>, IQueryable<Report>>? modifierFunc = null)
    {
        month = month.TrimToMonth();
        var foundOwner = _dbContext.Users
            .FirstOrDefault(x => x.Name == username);
        if (foundOwner == null)
            throw new NotFoundException();
        while (true)
        {
            var query = _dbContext.Reports.AsQueryable();
            if (modifierFunc != null)
                query = modifierFunc(query);
            var foundReport = query.AsNoTracking()
                .FirstOrDefault(x => x.OwnerId == foundOwner.Id && x.Month == month);
            if (foundReport != null)
                return foundReport;
            var newReport = new Report { OwnerId = foundOwner.Id, Month = month };
            _dbContext.Reports.Add(newReport);
            _dbContext.SaveChanges();
        }
    }

    public List<Report> FindReportsByProject(string projectCode, Func<IQueryable<Report>, IQueryable<Report>>? modifierFunc = null)
    {
        var query = _dbContext.Reports.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        var foundReports = query.AsNoTracking()
            .Include(x => x.ReportEntries)!
                .ThenInclude(x => x.Project)
            .Where(x => x.ReportEntries!.Any(y => y.ProjectCode == projectCode));
        return foundReports
            .Distinct()
            .ToList();
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
        // TODO: Throw if report has been frozen
        var foundReportEntry = _dbContext.ReportEntries
            .FirstOrDefault(x => x.Id == reportEntryId);
        if (foundReportEntry == null)
            throw new NotFoundException();
        _dbContext.ReportEntries.Remove(foundReportEntry);
        _dbContext.SaveChanges();
    }

    public ReportEntry? FindReportEntryById(int reportEntryId, Func<IQueryable<ReportEntry>, IQueryable<ReportEntry>>? modifierFunc = null)
    {
        var query = _dbContext.ReportEntries.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .FirstOrDefault(x => x.Id == reportEntryId);
    }

    public List<ReportEntry> FindReportEntriesByUsernameAndDay(string username, DateTime day, Func<IQueryable<ReportEntry>, IQueryable<ReportEntry>>? modifierFunc = null)
    {
        var query = _dbContext.ReportEntries.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .Include(x => x.Report)
                .ThenInclude(x => x!.Owner)
            .Where(x => x.Date == day.Date && x.Report!.Owner!.Name == username)
            .ToList();
    }

    public List<ReportEntry> FindReportEntriesByUsernameAndMonth(string username, DateTime month, Func<IQueryable<ReportEntry>, IQueryable<ReportEntry>>? modifierFunc = null)
    {
        var query = _dbContext.ReportEntries.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .Include(x => x.Report)
                .ThenInclude(x => x!.Owner)
            .Where(x => x.Date.TrimToMonth() == month.TrimToMonth() && x.Report!.Owner!.Name == username)
            .ToList();
    }

    public void UpdateReportEntry(ReportEntry reportEntry)
    {
        var updatedReportEntry = _dbContext.ReportEntries.Update(reportEntry);
        _dbContext.SaveChanges();
        updatedReportEntry.State = EntityState.Detached;
    }

    public AcceptedTime? FindAcceptedTimeByReportIdAndProjectCode(int reportId, string projectCode, Func<IQueryable<AcceptedTime>, IQueryable<AcceptedTime>>? modifierFunc = null)
    {
        var query = _dbContext.AcceptedTime.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .FirstOrDefault(x => x.ReportId == reportId && x.ProjectCode == projectCode);
    }

    public void SetAcceptedTime(AcceptedTime acceptedTime)
    {
        var foundAccepted = _dbContext.AcceptedTime
            .AsNoTracking()
            .FirstOrDefault(x => x.ReportId == acceptedTime.ReportId && x.ProjectCode == acceptedTime.ProjectCode);
        if (foundAccepted != null)
            _dbContext.AcceptedTime.Update(acceptedTime);
        else
            _dbContext.AcceptedTime.Add(acceptedTime);
        _dbContext.SaveChanges();
    }

    public byte[] GetTimestampForAcceptedTime(AcceptedTime acceptedTime) =>
        _dbContext.Entry(acceptedTime).Entity.Timestamp;

    public Category? FindCategoryByProjectCodeAndCode(string projectCode, string categoryCode, Func<IQueryable<Category>, IQueryable<Category>>? modifierFunc = null)
    {
        var query = _dbContext.Categories.AsQueryable();
        if (modifierFunc != null)
            query = modifierFunc(query);
        return query.AsNoTracking()
            .FirstOrDefault(x => x.ProjectCode == projectCode && x.Code == categoryCode);
    }

    public void AddCategory(Category category)
    {
        var addedCategory = _dbContext.Categories.Add(category);
        _dbContext.SaveChanges();
        addedCategory.State = EntityState.Detached;
    }
}
