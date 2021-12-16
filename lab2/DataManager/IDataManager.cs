using Trs.Models.DomainModels;

namespace Trs.DataManager;

public interface IDataManager
{
    int AddUser(User user);
    User? FindUserByName(string name);
    HashSet<User> GetAllUsers();

    string AddProject(Project project);
    Project? FindProjectByCode(string code);
    HashSet<Project> FindProjectsByManager(string managerName);
    HashSet<Project> GetAllProjects();
    void UpdateProject(Project project);

    Report FindOrCreateReportByUsernameAndMonth(string username, DateTime month);
    HashSet<Report> FindReportsByProject(string projectCode);
    void FreezeReportById(int reportId);

    int AddReportEntry(ReportEntry reportEntry);
    void DeleteReportEntryById(int reportEntryId);
    ReportEntry? FindReportEntryById(int reportEntryId);
    HashSet<ReportEntry> FindReportEntriesByUsernameAndDay(string username, DateTime day);
    HashSet<ReportEntry> FindReportEntriesByUsernameAndMonth(string username, DateTime month);
    void UpdateReportEntry(ReportEntry reportEntry);

    void SetAcceptedTime(AcceptedTime acceptedTime);

    Category? FindCategoryByProjectCodeAndCode(string projectCode, string categoryCode);
}
