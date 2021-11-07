using System;
using System.Collections.Generic;
using TRS.Models.DomainModels;

namespace TRS.DataManager
{
    public interface IDataManager
    {
        User AddUser(User user);
        User FindUserByName(string name);
        HashSet<User> GetAllUsers();

        Project AddProject(Project project);
        Project FindProjectByCode(string code);
        HashSet<Project> FindProjectsByManager(User manager);
        HashSet<Project> GetAllProjects();
        Project UpdateProject(Project project);

        ReportWithoutEntries FindReportByUserAndMonth(User user, DateTime month);
        HashSet<Report> FindReportByProject(Project project);
        ReportWithoutEntries UpdateReport(ReportWithoutEntries report);

        ReportEntry AddReportEntry(User user, ReportEntry reportEntry);
        void DeleteReportEntry(User user, DateTime day, int indexForDate);
        ReportEntry FindReportEntryByDayAndIndex(User user, DateTime day, int indexForDate);
        HashSet<ReportEntry> FindReportEntriesByDay(User user, DateTime day);
        HashSet<ReportEntry> FindReportEntriesByMonth(User user, DateTime month);
        ReportEntry UpdateReportEntry(User user, ReportEntry reportEntry);
    }
}
