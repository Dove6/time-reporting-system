using System;
using System.Collections.Generic;
using TRS.Models.DomainModels;

namespace TRS.DataManager
{
    public interface IDataManager
    {
        void AddUser(UserModel user);
        UserModel FindUserByName(string name);

        Dictionary<string, UserModel> GetAllUsers();

        void AddProject(ProjectModel project);
        ProjectModel FindProjectByCode(string code);
        List<ProjectModel> GetAllProjects();
        void UpdateProject(ProjectModel project);

        void AddReportEntry(ReportEntryModel reportEntry);
        void DeleteReportEntry(ReportEntryModel reportEntry);
        ReportEntryModel FindReportEntryByIdForUserInMonth(int id, string user, DateTime month);
        ReportModel GetReportForUserInMonth(string user, DateTime month);
        void UpdateReportEntry(ReportEntryModel reportEntry);
    }
}
