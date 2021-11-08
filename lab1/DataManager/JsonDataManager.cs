using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using TRS.DataManager.Exceptions;
using TRS.DataManager.JsonHelpers;
using TRS.Models.DomainModels;
using TRS.Models.JsonModels;
using Report = TRS.Models.DomainModels.Report;
using ReportEntry = TRS.Models.DomainModels.ReportEntry;

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
            userList ??= new List<string>();
            var mappedUserList = userList.Select(x => new User { Name = x }).ToHashSet();
            return mappedUserList;
        }

        private HashSet<Project> ReadAllProjects()
        {
            if (!File.Exists(ProjectListPath))
                return new HashSet<Project>();

            var serializedProjectList = File.ReadAllText(ProjectListPath);
            var projectList =
                JsonSerializer.Deserialize<ActivityList>(serializedProjectList, SerializerOptions);
            projectList ??= new ActivityList();
            var mappedProjectList = _mapper.Map<HashSet<Project>>(projectList.Activities);
            return mappedProjectList;
        }

        private HashSet<Report> ReadAllReports(string username = null, DateTime? month = null)
        {
            var readReports = new HashSet<Report>();
            var userSearchPattern = username ?? "*?";
            var monthSearchPattern = month?.ToString(MonthDateFormat) ?? "????-??";
            var searchPattern = $"{userSearchPattern}-{monthSearchPattern}.json";
            foreach (var reportPath in Directory.GetFiles(StoragePath, searchPattern))
            {
                var serializedReport = File.ReadAllText(reportPath);
                var report = JsonSerializer.Deserialize<Models.JsonModels.Report>(serializedReport, SerializerOptions);
                report ??= new Models.JsonModels.Report();
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
            var mappedProjectList = new ActivityList
            {
                Activities = _mapper.Map<List<Activity>>(projectList)
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

        public void AddUser(User user)
        {
            var userSet = GetAllUsers();
            if (!userSet.Add(user))
                throw new AlreadyExistingException();
            WriteAllUsers(userSet);
        }

        public User FindUserByName(string name)
        {
            return GetAllUsers().FirstOrDefault(x => x.Name == name);
        }

        public HashSet<User> GetAllUsers()
        {
            return ReadAllUsers();
        }

        public void AddProject(Project project)
        {
            var projectSet = GetAllProjects();
            if (!projectSet.Add(project))
                throw new AlreadyExistingException();
            WriteAllProjects(projectSet);
        }

        public Project FindProjectByCode(string code) =>
            ReadAllProjects().FirstOrDefault(x => x.Code == code);

        public HashSet<Project> FindProjectsByManager(string managerName) =>
            ReadAllProjects().Where(x => x.Manager == managerName).ToHashSet();

        public HashSet<Project> GetAllProjects()
        {
            var mappedProjectList = ReadAllProjects();
            return new HashSet<Project>(mappedProjectList);
        }

        public void UpdateProject(Project project)
        {
            var projectSet = ReadAllProjects();
            if (projectSet.Remove(project))
                throw new NotFoundException();
            projectSet.Add(project);
            WriteAllProjects(projectSet);
        }

        public Report FindReportByUserAndMonth(string username, DateTime month) =>
            ReadAllReports(username, month).FirstOrDefault() ?? new Report { Owner = username, Month = month };

        public HashSet<Report> FindReportByProject(Project project) =>
            ReadAllReports().Where(x => x.Entries.Any(y => y.Code == project.Code)).ToHashSet();

        public void FreezeReport(string username, DateTime month)
        {
            var report = FindReportByUserAndMonth(username, month);
            report.Frozen = true;
            WriteReportForUserInMonth(report);
        }

        public void AddReportEntry(string username, ReportEntry reportEntry)
        {
            var report = FindReportByUserAndMonth(username, reportEntry.Date);
            reportEntry.MonthlyIndex = report.Entries.Count;
            if (!report.Entries.Add(reportEntry))
                throw new AlreadyExistingException();
            WriteReportForUserInMonth(report);
        }

        public void DeleteReportEntry(string username, DateTime day, int id)
        {
            var report = ReadAllReports(username, day).FirstOrDefault();
            if (report == null)
                throw new NotFoundException();
            if (report.Entries.RemoveWhere(x => x.Date == day.Date && x.MonthlyIndex == id) == 0)
                throw new NotFoundException();
            WriteReportForUserInMonth(report);
        }

        public ReportEntry FindReportEntryByDayAndIndex(string username, DateTime day, int id) =>
            FindReportByUserAndMonth(username, day).Entries.FirstOrDefault(x => x.Date == day.Date && x.MonthlyIndex == id);

        public HashSet<ReportEntry> FindReportEntriesByDay(string username, DateTime day) =>
            FindReportByUserAndMonth(username, day).Entries.Where(x => x.Date == day.Date).ToHashSet();

        public HashSet<ReportEntry> FindReportEntriesByMonth(string username, DateTime month) =>
            ReadAllReports(username, month).FirstOrDefault()?.Entries.ToHashSet() ?? new HashSet<ReportEntry>();

        public void UpdateReportEntry(string username, DateTime day, int id, ReportEntry reportEntry)
        {
            var report = ReadAllReports(username, reportEntry.Date).FirstOrDefault();
            if (report == null)
                throw new NotFoundException();
            if (!report.Entries.Remove(reportEntry))
                throw new NotFoundException();
            report.Entries.Add(reportEntry);
            WriteReportForUserInMonth(report);
        }

        public void AddAcceptedTime(string username, DateTime month, AcceptedTime acceptedTime)
        {
            var report = FindReportByUserAndMonth(username, month);
            if (!report.Accepted.Add(acceptedTime))
                throw new AlreadyExistingException();
            WriteReportForUserInMonth(report);
        }

        public void UpdateAcceptedTime(string username, DateTime month, AcceptedTime acceptedTime)
        {
            var report = FindReportByUserAndMonth(username, month);
            if (!report.Accepted.Remove(acceptedTime))
                throw new NotFoundException();
            report.Accepted.Add(acceptedTime);
            WriteReportForUserInMonth(report);
        }
    }
}
