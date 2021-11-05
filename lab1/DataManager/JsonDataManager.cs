﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using TRS.DataManager.JsonHelpers;
using TRS.Models.DomainModels;

namespace TRS.DataManager
{
    public class JsonDataManager : IDataManager
    {
        private const string StoragePath = "storage";

        private const string UserListFilename = "users.json";
        private const string ProjectListFilename = "activity.json";
        private const string MonthDateFormat = "yyyy-MM";

        private static readonly string UserListPath = Path.Combine(StoragePath, UserListFilename);
        private static readonly string ProjectListPath = Path.Combine(StoragePath, ProjectListFilename);
        private static string UserMonthEntryListPath(string username, DateTime month) =>
            Path.Combine(StoragePath, $"{username}-{month.ToString(MonthDateFormat)}.json");

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            WriteIndented = true
        };

        private readonly IMapper _mapper;

        public JsonDataManager(IMapper mapper)
        {
            _mapper = mapper;
        }

        private HashSet<User> ReadAllUsers()
        {
            if (!File.Exists(UserListPath))
                return new HashSet<User>();

            var data = File.ReadAllText(UserListPath);
            var userList = JsonSerializer.Deserialize<List<string>>(data, SerializerOptions);
            var mappedUserList = userList.Select(x => new User(x));
            return new HashSet<User>(mappedUserList);
        }

        private HashSet<Project> ReadAllProjects()
        {
            if (!File.Exists(ProjectListPath))
                return new HashSet<Project>();

            var data = File.ReadAllText(ProjectListPath);
            var projectList = JsonSerializer.Deserialize<Models.JsonModels.ProjectListModel>(data, SerializerOptions);
            var mappedProjectList = _mapper.Map<ProjectList>(projectList);
            return mappedProjectList.Activities;
        }

        private List<ReportEntry> ReadAllReportEntries()
        {
            var allReportEntries = new List<ReportEntry>();
            foreach (var reportPath in Directory.GetFiles(StoragePath, "*?-*?-*?.json"))
            {
                var user = ParseFilename(reportPath).Owner;
                var data = File.ReadAllText(reportPath);
                var report = JsonSerializer.Deserialize<Models.JsonModels.ReportModel>(data, SerializerOptions);
                var mappedReport = _mapper.Map<Report>(report);
                foreach (var entry in mappedReport.Entries)
                    entry.Owner = user;
                allReportEntries.AddRange(mappedReport.Entries);
            }
            return allReportEntries;
        }

        private Report ReadReportForUserInMonth(User user, DateTime month)
        {
            var reportPath = UserMonthEntryListPath(user.Name, month);
            if (!File.Exists(reportPath))
                return new Report(user, month);

            var data = File.ReadAllText(reportPath);
            var report = JsonSerializer.Deserialize<Models.JsonModels.ReportModel>(data, SerializerOptions);
            report.Filename = reportPath;
            var mappedReport = _mapper.Map<Report>(report);
            foreach (var entry in mappedReport.Entries)
                entry.Owner = user;
            return mappedReport;
        }

        private void WriteAllUsers(HashSet<User> users)
        {
            var mappedUserList = users.Select(x => x.Name).ToList();
            var data = JsonSerializer.Serialize(mappedUserList, SerializerOptions);
            File.WriteAllText(UserListPath, data);
        }

        private void WriteAllProjects(HashSet<Project> projectList)
        {
            var mappedProjectList = new Models.JsonModels.ProjectListModel
            {
                Activities = _mapper.Map<List<Models.JsonModels.Project>>(projectList)
            };
            var data = JsonSerializer.Serialize(mappedProjectList, SerializerOptions);
            File.WriteAllText(ProjectListPath, data);
        }

        private void WriteReportForUserInMonth(Report report)
        {
            var mappedReport = _mapper.Map<Models.JsonModels.ReportModel>(report);
            var data = JsonSerializer.Serialize(mappedReport, SerializerOptions);
            var reportPath = UserMonthEntryListPath(report.Owner.Name, report.Month);
            File.WriteAllText(reportPath, data);
        }

        public User AddUser(User user)
        {
            var userSet = GetAllUsers();
            userSet.Add(user);
            WriteAllUsers(userSet);
            return user;
        }

        public User FindUserByName(string name)
        {
            return GetAllUsers().FirstOrDefault(x => x.Name == name);
        }

        public HashSet<User> GetAllUsers()
        {
            return ReadAllUsers();
        }

        public Project AddProject(Project project)
        {
            var projectSet = GetAllProjects();
            projectSet.Add(project);
            WriteAllProjects(projectSet);
            return project;
        }

        public Project FindProjectByCode(string code)
        {
            return ReadAllProjects().FirstOrDefault(x => x.Code == code);
        }

        public HashSet<Project> GetAllProjects()
        {
            var mappedProjectList = ReadAllProjects();
            return new HashSet<Project>(mappedProjectList);
        }

        public Project UpdateProject(Project project)
        {
            var projectSet = ReadAllProjects();
            projectSet.Remove(project);
            projectSet.Add(project);
            WriteAllProjects(projectSet);
            return project;
        }

        public Report AddReport(Report report)
        {
            WriteReportForUserInMonth(report);
            return report;
        }

        public Report FindReportByUserAndMonth(User user, DateTime month)
        {
            return ReadReportForUserInMonth(user, month);
        }

        public Report UpdateReport(Report report)
        {
            WriteReportForUserInMonth(report);
            return report;
        }

        private static ParsedFilename ParseFilename(string reportPath)
        {
            var filename = new FileInfo(reportPath).Name;
            var userRegex = new Regex(@"^(.+?)-(\d{4}-\d{2})\.json$");
            return new ParsedFilename
            {
                Owner = new User(userRegex.Match(filename).Groups[1].Value),
                Month = DateTime.ParseExact(userRegex.Match(filename).Groups[2].Value, "yyyy-MM", CultureInfo.InvariantCulture)
            };
        }

        public static User GetUserFromFilename(string reportPath) => ParseFilename(reportPath).Owner;
        public static DateTime GetMonthFromFilename(string reportPath) => ParseFilename(reportPath).Month;

        private class ParsedFilename
        {
            public User Owner;
            public DateTime Month;
        }
    }
}