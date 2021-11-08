using System;
using System.Collections.Generic;
using TRS.Models.DomainModels;

namespace TRS.DataManager
{
    public interface IDataManager
    {
        void AddUser(User user);
        User FindUserByName(string name);
        HashSet<User> GetAllUsers();

        void AddProject(Project project);
        Project FindProjectByCode(string code);
        HashSet<Project> FindProjectsByManager(string managerName);
        HashSet<Project> GetAllProjects();
        void UpdateProject(Project project);

        Report FindReportByUserAndMonth(string username, DateTime month);
        HashSet<Report> FindReportByProject(Project project);
        void FreezeReport(string username, DateTime month);

        void AddReportEntry(string username, ReportEntry reportEntry);
        void DeleteReportEntry(string username, DateTime day, int id);
        ReportEntry FindReportEntryByDayAndIndex(string username, DateTime day, int id);
        HashSet<ReportEntry> FindReportEntriesByDay(string username, DateTime day);
        HashSet<ReportEntry> FindReportEntriesByMonth(string username, DateTime month);
        void UpdateReportEntry(string username, DateTime day, int id, ReportEntry reportEntry);

        void AddAcceptedTime(string username, DateTime month, AcceptedTime acceptedTime);
        void UpdateAcceptedTime(string username, DateTime month, AcceptedTime acceptedTime);
    }
}
