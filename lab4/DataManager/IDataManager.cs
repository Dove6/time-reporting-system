using Trs.Models.DomainModels;

namespace Trs.DataManager;

public interface IDataManager
{
    AcceptedTime? FindAcceptedTimeByUsernameAndReportMonthAndProjectCode(string username, string month, string projectCode,
        Func<IQueryable<AcceptedTime>, IQueryable<AcceptedTime>>? modifierFunc = null);

    void SetAcceptedTime(AcceptedTime acceptedTime);

    Category? FindCategoryByProjectCodeAndCode(string projectCode, string categoryCode,
        Func<IQueryable<Category>, IQueryable<Category>>? modifierFunc = null);
    void AddCategory(Category category);

    Project? FindProjectByCode(string code, Func<IQueryable<Project>, IQueryable<Project>>? modifierFunc = null);

    List<Project> FindProjectsByManager(string managerName,
        Func<IQueryable<Project>, IQueryable<Project>>? modifierFunc = null);

    List<Project> GetAllProjects(Func<IQueryable<Project>, IQueryable<Project>>? modifierFunc = null);

    void AddProject(Project project);

    void UpdateProject(Project project);

    Report FindOrCreateReportByUsernameAndMonth(string username, string month,
        Func<IQueryable<Report>, IQueryable<Report>>? modifierFunc = null);

    List<Report> FindReportsByProject(string projectCode,
        Func<IQueryable<Report>, IQueryable<Report>>? modifierFunc = null);

    void FreezeReportByOwnerIdAndMonth(int ownerId, string month);

    ReportEntry? FindReportEntryById(int reportEntryId,
        Func<IQueryable<ReportEntry>, IQueryable<ReportEntry>>? modifierFunc = null);

    List<ReportEntry> FindReportEntriesByUsernameAndDate(string username, DateTime date,
        Func<IQueryable<ReportEntry>, IQueryable<ReportEntry>>? modifierFunc = null);

    List<ReportEntry> FindReportEntriesByUsernameAndMonth(string username, string month,
        Func<IQueryable<ReportEntry>, IQueryable<ReportEntry>>? modifierFunc = null);

    int AddReportEntry(ReportEntry reportEntry);

    void DeleteReportEntryById(int reportEntryId);

    void UpdateReportEntry(ReportEntry reportEntry);

    User? FindUserById(int id, Func<IQueryable<User>, IQueryable<User>>? modifierFunc = null);

    User? FindUserByName(string name, Func<IQueryable<User>, IQueryable<User>>? modifierFunc = null);

    List<User> GetAllUsers(Func<IQueryable<User>, IQueryable<User>>? modifierFunc = null);

    int AddUser(User user);
}
