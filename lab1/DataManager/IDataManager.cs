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

        Report FindReportByUserAndMonth(User user, DateTime month);
        HashSet<Report> FindReportByProject(Project project);
        void FreezeReport(User user, DateTime month);

        ReportEntry AddReportEntry(User user, ReportEntry reportEntry);
        void DeleteReportEntry(User user, DateTime day, int id);
        ReportEntry FindReportEntryByDayAndIndex(User user, DateTime day, int id);
        HashSet<ReportEntry> FindReportEntriesByDay(User user, DateTime day);
        HashSet<ReportEntry> FindReportEntriesByMonth(User user, DateTime month);
        ReportEntry UpdateReportEntry(User user, DateTime day, int id, ReportEntry reportEntry);

        AcceptedTime AddAcceptedTime(User user, DateTime month, AcceptedTime acceptedTime);
        AcceptedTime UpdateAcceptedTime(User user, DateTime month, AcceptedTime acceptedTime);
    }
}
