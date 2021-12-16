using AutoMapper;
using TRS.Extensions;
using TRS.Models.DomainModels;

namespace TRS.DataManager;

public class DbDataManager : IDataManager
{
    private readonly TrsDbContext _dbContext;
    private readonly IMapper _mapper;

    public DbDataManager(TrsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public void AddUser(User user)
    {
        var mappedUser = _mapper.Map<Trs.Models.DbModels.User>(user);
        _dbContext.Users.Add(mappedUser);
        _dbContext.SaveChanges();
    }

    public User? FindUserByName(string name)
    {
        var foundUser = _dbContext.Users.FirstOrDefault(x => x.Name == name);
        return _mapper.Map<User>(foundUser);
    }

    public HashSet<User> GetAllUsers()
    {
        return _mapper.Map<HashSet<User>>(_dbContext.Users.ToHashSet());
    }

    public void AddProject(Project project)
    {
        var mappedProject = _mapper.Map<Trs.Models.DbModels.Project>(project);
        _dbContext.Projects.Add(mappedProject);
        _dbContext.SaveChanges();
    }

    public Project? FindProjectByCode(string code)
    {
        var foundProject = _dbContext.Projects.FirstOrDefault(x => x.Code == code);
        return _mapper.Map<Project>(foundProject);
    }

    public HashSet<Project> FindProjectsByManager(string managerName)
    {
        var foundManager = _dbContext.Users.FirstOrDefault(x => x.Name == managerName);
        if (foundManager == null)
            return new HashSet<Project>();
        var foundProjects = _dbContext.Projects.Where(x => x.ManagerId == foundManager.Id).ToHashSet();
        return _mapper.Map<HashSet<Project>>(foundProjects);
    }

    public HashSet<Project> GetAllProjects()
    {
        return _mapper.Map<HashSet<Project>>(_dbContext.Projects.ToHashSet());
    }

    public void UpdateProject(Project project)
    {
        var mappedProject = _mapper.Map<Trs.Models.DbModels.Project>(project);
        _dbContext.Projects.Update(mappedProject);
        _dbContext.SaveChanges();
    }

    public Report? FindReportByUserAndMonth(string username, DateTime month)
    {
        var foundOwner = _dbContext.Users.FirstOrDefault(x => x.Name == username);
        if (foundOwner == null)
            return null;
        var foundReport = _dbContext.Reports.FirstOrDefault(x => x.OwnerId == foundOwner.Id && x.Month == month.TrimToMonth());
        return _mapper.Map<Report>(foundReport);
    }

    public HashSet<Report> FindReportsByProject(string projectCode)
    {
        var foundProject = _dbContext.Projects.FirstOrDefault(x => x.Code == projectCode);
        if (foundProject == null)
            return new HashSet<Report>();
        return _mapper.Map<HashSet<Report>>(foundProject.ReportEntries.Select(x => x.Project).Distinct().ToHashSet());
    }

    public void FreezeReport(string username, DateTime month)
    {
        var foundOwner = _dbContext.Users.FirstOrDefault(x => x.Name == username);
        if (foundOwner == null)
            return;
        var foundReport = _dbContext.Reports.FirstOrDefault(x => x.OwnerId == foundOwner.Id && x.Month == month.TrimToMonth());
        if (foundReport == null)
            return;
        foundReport.Frozen = true;
        _dbContext.Update(foundReport);
        _dbContext.SaveChanges();
    }

    public void AddReportEntry(string username, ReportEntry reportEntry)
    {
        var mappedReportEntry = _mapper.Map<Trs.Models.DbModels.ReportEntry>(reportEntry);
        var foundOwner = _dbContext.Users.FirstOrDefault(x => x.Name == username);
        if (foundOwner == null)
            return;
        var foundReport = _dbContext.Reports.FirstOrDefault(x => x.OwnerId == foundOwner.Id && x.Month == reportEntry.Month.TrimToMonth());
        if (foundReport == null)
            return;
        mappedReportEntry.ReportId = foundReport.Id;
        var foundProject = _dbContext.Projects.FirstOrDefault(x => x.Code == reportEntry.Code);
        if (foundProject == null)
            return;
        mappedReportEntry.ProjectCode = foundProject.Code;
        if (!string.IsNullOrEmpty(reportEntry.Subcode))
        {
            var foundCategory = _dbContext.Categories.FirstOrDefault(x => x.Name == reportEntry.Subcode);
            if (foundCategory == null)
                return;
            mappedReportEntry.CategoryId = foundCategory.Id;
        }
        _dbContext.ReportEntries.Add(mappedReportEntry);
        _dbContext.SaveChanges();
    }

    public void DeleteReportEntry(string username, DateTime day, int id)
    {
        var foundReportEntry = _dbContext.ReportEntries
            .Where(x => x.Date == day.Date)
            .OrderBy(x => x.Id)
            .Skip(id)
            .FirstOrDefault();
        if (foundReportEntry != null)
            _dbContext.ReportEntries.Remove(foundReportEntry);
    }

    public ReportEntry? FindReportEntryByDayAndIndex(string username, DateTime day, int id)
    {
        var foundReportEntry = _dbContext.ReportEntries
            .Where(x => x.Date == day.Date)
            .OrderBy(x => x.Id)
            .Skip(id)
            .FirstOrDefault();
        return _mapper.Map<ReportEntry>(foundReportEntry);
    }

    public HashSet<ReportEntry> FindReportEntriesByDay(string username, DateTime day)
    {
        var foundReportEntries = _dbContext.ReportEntries
            .Where(x => x.Date == day.Date)
            .ToHashSet();
        return _mapper.Map<HashSet<ReportEntry>>(foundReportEntries);
    }

    public HashSet<ReportEntry> FindReportEntriesByMonth(string username, DateTime month)
    {
        var foundReportEntries = _dbContext.ReportEntries
            .Where(x => x.Date.TrimToMonth() == month.TrimToMonth())
            .ToHashSet();
        return _mapper.Map<HashSet<ReportEntry>>(foundReportEntries);
    }

    public void UpdateReportEntry(string username, DateTime day, int id, ReportEntry reportEntry)
    {
        var foundReportEntry = _dbContext.ReportEntries
            .Where(x => x.Date == day.Date)
            .OrderBy(x => x.Id)
            .Skip(id)
            .FirstOrDefault();
        if (foundReportEntry == null)
            return;
        var mappedReportEntry = _mapper.Map<Trs.Models.DbModels.ReportEntry>(reportEntry);
        mappedReportEntry.Id = foundReportEntry.Id;
        mappedReportEntry.ReportId = foundReportEntry.ReportId;
        mappedReportEntry.ProjectCode = foundReportEntry.ProjectCode;
        mappedReportEntry.CategoryId = foundReportEntry.CategoryId;
        _dbContext.Update(mappedReportEntry);
        _dbContext.SaveChanges();
    }

    public void SetAcceptedTime(string username, DateTime month, AcceptedTime acceptedTime)
    {
        var foundOwner = _dbContext.Users.FirstOrDefault(x => x.Name == username);
        if (foundOwner == null)
            return;
        var foundReport = _dbContext.Reports.FirstOrDefault(x => x.OwnerId == foundOwner.Id && x.Month == month.TrimToMonth());
        if (foundReport == null)
            return;
        var foundProject = _dbContext.Projects.FirstOrDefault(x => x.Code == acceptedTime.Code);
        if (foundProject == null)
            return;
        var mappedAcceptedTime = _mapper.Map<Trs.Models.DbModels.AcceptedTime>(acceptedTime);
        mappedAcceptedTime.ProjectCode = foundProject.Code;
        mappedAcceptedTime.ReportId = foundReport.Id;
        if (_dbContext.AcceptedTime.FirstOrDefault(x => x.ReportId == foundReport.Id && x.ProjectCode == foundProject.Code) != null)
            _dbContext.AcceptedTime.Update(mappedAcceptedTime);
        else
            _dbContext.AcceptedTime.Add(mappedAcceptedTime);
        _dbContext.SaveChanges();
    }
}
