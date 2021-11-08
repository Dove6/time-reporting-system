using System;
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

        public static string GetReportFilename(string username, DateTime month) =>
            $"{username}-{month.ToString(MonthDateFormat)}.json";

        private static string GetReportPath(string username, DateTime month) =>
            Path.Combine(StoragePath, GetReportFilename(username, month));

        private static (string, DateTime) ParseReportFilename(string reportPath)
        {
            var filename = new FileInfo(reportPath).Name;
            var userRegex = new Regex(@"^(.+?)-(\d{4}-\d{2})\.json$", RegexOptions.Compiled);
            return (
                userRegex.Match(filename).Groups[1].Value,
                DateTime.ParseExact(userRegex.Match(filename).Groups[2].Value,
                    MonthDateFormat,
                    CultureInfo.InvariantCulture)
            );
        }

        public static string GetOwnerFromReportFilename(string reportPath) => ParseReportFilename(reportPath).Item1;

        public static DateTime GetMonthFromReportFilename(string reportPath) => ParseReportFilename(reportPath).Item2;

        private static HashSet<User> ReadAllUsers()
        {
            if (!File.Exists(UserListPath))
                return new HashSet<User>();

            var serializedUserList = File.ReadAllText(UserListPath);
            var userList = JsonSerializer.Deserialize<List<string>>(serializedUserList, SerializerOptions);
            var mappedUserList = userList.Select(x => new User { Name = x }).ToHashSet();
            return mappedUserList;
        }

        private HashSet<Project> ReadAllProjects()
        {
            if (!File.Exists(ProjectListPath))
                return new HashSet<Project>();

            var serializedProjectList = File.ReadAllText(ProjectListPath);
            var projectList =
                JsonSerializer.Deserialize<Models.JsonModels.ActivityList>(serializedProjectList, SerializerOptions);
            var mappedProjectList = _mapper.Map<HashSet<Project>>(projectList.Activities);
            return mappedProjectList;
        }

        private HashSet<Report> ReadAllReports(User user = null, DateTime? month = null)
        {
            var readReports = new HashSet<Report>();
            var userSearchPattern = user != null ? user.Name : "*?";
            var monthSearchPattern = month?.ToString(MonthDateFormat) ?? "????-??";
            var searchPattern = $"{userSearchPattern}-{monthSearchPattern}.json";
            foreach (var reportPath in Directory.GetFiles(StoragePath, searchPattern))
            {
                var serializedReport = File.ReadAllText(reportPath);
                var report = JsonSerializer.Deserialize<Models.JsonModels.Report>(serializedReport, SerializerOptions);
                report.Filename = reportPath;
                var mappedReport = _mapper.Map<Report>(report);
                mappedReport.Entries = report.Entries.Select((x, i) =>
                {
                    var mappedEntry = _mapper.Map<ReportEntry>(x);
                    mappedEntry.MonthlyIndex = i;
                    return mappedEntry;
                }).ToHashSet();
                readReports.Add(mappedReport);
            }
            return readReports;
        }

        private static void WriteAllUsers(HashSet<User> users)
        {
            var mappedUserList = users.Select(x => x.Name)
                .OrderBy(x => x).ToList();
            var data = JsonSerializer.Serialize(mappedUserList, SerializerOptions);
            File.WriteAllText(UserListPath, data);
        }

        private void WriteAllProjects(HashSet<Project> projectList)
        {
            var mappedProjectList = new Models.JsonModels.ActivityList
            {
                Activities = _mapper.Map<List<Models.JsonModels.Activity>>(projectList)
            };
            mappedProjectList.Activities.Sort((x, y) =>
                string.Compare(x.Code, y.Code, StringComparison.InvariantCulture));
            var data = JsonSerializer.Serialize(mappedProjectList, SerializerOptions);
            File.WriteAllText(ProjectListPath, data);
        }

        private void WriteReportForUserInMonth(Report report)
        {
            var mappedReport = _mapper.Map<Models.JsonModels.Report>(report);
            var data = JsonSerializer.Serialize(mappedReport, SerializerOptions);
            var reportPath = GetReportPath(report.Owner, report.Month);
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

        public Project FindProjectByCode(string code) =>
            ReadAllProjects().FirstOrDefault(x => x.Code == code);

        public HashSet<Project> FindProjectsByManager(User manager) =>
            ReadAllProjects().Where(x => x.Manager == manager.Name).ToHashSet();

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

        public Report FindReportByUserAndMonth(User user, DateTime month) =>
            ReadAllReports(user, month).FirstOrDefault() ?? new Report { Owner = user.Name, Month = month };

        public HashSet<Report> FindReportByProject(Project project) =>
            ReadAllReports().Where(x => x.Entries.Any(y => y.Code == project.Code)).ToHashSet();

        public void FreezeReport(User user, DateTime month)
        {
            var report = FindReportByUserAndMonth(user, month);
            report.Frozen = true;
            WriteReportForUserInMonth(report);
        }

        public ReportEntry AddReportEntry(User user, ReportEntry reportEntry)
        {
            var report = FindReportByUserAndMonth(user, reportEntry.Date);
            if (report == null)
                return null;
            reportEntry.MonthlyIndex = report.Entries.Count;
            report.Entries.Add(reportEntry);
            WriteReportForUserInMonth(report);
            return reportEntry;
        }

        public void DeleteReportEntry(User user, DateTime day, int id)
        {
            var report = ReadAllReports(user, day).FirstOrDefault();
            if (report == null)
                return;
            report.Entries.RemoveWhere(x => x.Date == day.Date && x.MonthlyIndex == id);
            WriteReportForUserInMonth(report);
        }

        public ReportEntry FindReportEntryByDayAndIndex(User user, DateTime day, int id) =>
            FindReportByUserAndMonth(user, day).Entries.FirstOrDefault(x => x.Date == day.Date && x.MonthlyIndex == id);

        public HashSet<ReportEntry> FindReportEntriesByDay(User user, DateTime day) =>
            FindReportByUserAndMonth(user, day)?.Entries.Where(x => x.Date == day.Date).ToHashSet() ?? new HashSet<ReportEntry>();

        public HashSet<ReportEntry> FindReportEntriesByMonth(User user, DateTime month) =>
            ReadAllReports(user, month).FirstOrDefault()?.Entries.ToHashSet() ?? new HashSet<ReportEntry>();

        public ReportEntry UpdateReportEntry(User user, DateTime day, int id, ReportEntry reportEntry)
        {
            var report = ReadAllReports(user, reportEntry.Date).FirstOrDefault();
            if (report == null)
                return null;
            if (report.Entries.Remove(reportEntry))
                report.Entries.Add(reportEntry);
            WriteReportForUserInMonth(report);
            return reportEntry;
        }

        public AcceptedTime AddAcceptedTime(User user, DateTime month, AcceptedTime acceptedTime)
        {
            var report = FindReportByUserAndMonth(user, month);
            report.Accepted.Add(acceptedTime);
            WriteReportForUserInMonth(report);
            return acceptedTime;
        }

        public AcceptedTime UpdateAcceptedTime(User user, DateTime month, AcceptedTime acceptedTime)
        {
            var report = FindReportByUserAndMonth(user, month);
            report.Accepted.Remove(acceptedTime);
            report.Accepted.Add(acceptedTime);
            WriteReportForUserInMonth(report);
            return acceptedTime;
        }
    }
}
