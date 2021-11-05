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
        HashSet<Project> GetAllProjects();
        Project UpdateProject(Project project);

        Report AddReport(Report report);
        Report FindReportByUserAndMonth(User user, DateTime month);
        Report UpdateReport(Report report);
    }
}
