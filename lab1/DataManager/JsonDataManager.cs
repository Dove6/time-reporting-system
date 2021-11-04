using System;
using System.Collections.Generic;
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
        private static string UserMonthEntryListPath(string user, DateTime month) =>
            Path.Combine(StoragePath, $"{user}-{month.ToString(MonthDateFormat)}.json");

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            AllowTrailingCommas = true
        };

        private readonly IMapper _mapper;

        public JsonDataManager(IMapper mapper)
        {
            _mapper = mapper;
        }

        private Dictionary<string, UserModel> ReadAllUsers()
        {
            if (!File.Exists(UserListPath))
                return new Dictionary<string, UserModel>();

            var data = File.ReadAllText(UserListPath);
            var userList = JsonSerializer.Deserialize<List<string>>(data, SerializerOptions);
            return userList.Select(x => new UserModel { Name = x }).ToDictionary(x => x.Name, x => x);
        }

        private List<ProjectModel> ReadAllProjects()
        {
            if (!File.Exists(ProjectListPath))
                return new List<ProjectModel>();

            var data = File.ReadAllText(ProjectListPath);
            var projectList = JsonSerializer.Deserialize<TRS.Models.JsonModels.ProjectListModel>(data, SerializerOptions);
            return _mapper.Map<ProjectListModel>(projectList).Activities;
        }

        private List<ReportEntryModel> ReadAllReportEntries()
        {
            var allReportEntries = new List<ReportEntryModel>();
            foreach (var reportPath in Directory.GetFiles(StoragePath, "*?-*?-*?.json"))
            {
                var filename = new FileInfo(reportPath).Name;
                var userRegex = new Regex(@"^(.+?)-\d{4}-\d{2}\.json$");
                var user = userRegex.Match(filename).Groups[1].Value;
                var data = File.ReadAllText(reportPath);
                var report = JsonSerializer.Deserialize<TRS.Models.JsonModels.ReportModel>(data, SerializerOptions);
                var mappedReport = _mapper.Map<ReportModel>(report);
                foreach (var entry in mappedReport.Entries)
                    entry.Owner = user;
                allReportEntries.AddRange(mappedReport.Entries);
            }
            return allReportEntries;
        }

        private ReportModel ReadReportForUserInMonth(string user, DateTime month)
        {
            if (!File.Exists(UserMonthEntryListPath(user, month)))
                return new ReportModel
                {
                    Entries = new List<ReportEntryModel>(),
                    Accepted = new List<AcceptedSummaryModel>()
                };

            var data = File.ReadAllText(UserMonthEntryListPath(user, month));
            var report = JsonSerializer.Deserialize<TRS.Models.JsonModels.ReportModel>(data, SerializerOptions);
            var mappedReport = _mapper.Map<ReportModel>(report);
            foreach (var entry in mappedReport.Entries)
                entry.Owner = user;
            return mappedReport;
        }

        private void WriteAllUsers(Dictionary<string, UserModel> users)
        {
            var mappedUserList = users.Values.Select(x => x.Name).ToList();
            var data = JsonSerializer.Serialize(mappedUserList, SerializerOptions);
            File.WriteAllText(UserListPath, data);
        }

        private void WriteAllProjects(List<ProjectModel> projectList)
        {
            var mappedProjectList = new TRS.Models.JsonModels.ProjectListModel
            {
                Activities = _mapper.Map<List<TRS.Models.JsonModels.ProjectModel>>(projectList)
            };
            var data = JsonSerializer.Serialize(mappedProjectList, SerializerOptions);
            File.WriteAllText(UserListPath, data);
        }

        private void WriteReportForUserInMonth(ReportModel report, string user, DateTime month)
        {
            var mappedReport = _mapper.Map<TRS.Models.JsonModels.ReportModel>(report);
            var data = JsonSerializer.Serialize(mappedReport, SerializerOptions);
            File.WriteAllText($"{user}-{month.ToString(MonthDateFormat)}.json", data);
        }

        public void AddUser(UserModel user)
        {
            var users = ReadAllUsers();
            users.TryAdd(user.Name, user);
            WriteAllUsers(users);
        }

        public UserModel FindUserByName(string name)
        {
            return ReadAllUsers().TryGetValue(name, out var user) ? user : null;
        }

        public Dictionary<string, UserModel> GetAllUsers()
        {
            return ReadAllUsers();
        }

        public void AddProject(ProjectModel project)
        {
            throw new NotImplementedException();
        }

        public ProjectModel FindProjectByCode(string code)
        {
            return ReadAllProjects().FirstOrDefault(x => x.Code == code);
        }

        public List<ProjectModel> GetAllProjects()
        {
            return ReadAllProjects();
        }

        public void UpdateProject(ProjectModel project)
        {
            throw new NotImplementedException();
        }

        public void AddReportEntry(ReportEntryModel reportEntry)
        {
            throw new NotImplementedException();
        }

        public void DeleteReportEntry(ReportEntryModel reportEntry)
        {
            throw new NotImplementedException();
        }

        public ReportEntryModel FindReportEntryByIdForUserInMonth(int id, string user, DateTime month)
        {
            //return ReadReportEntriesForUserInMonth(user, month).ElementAt(id);
            return new();
        }

        public ReportModel GetReportForUserInMonth(string user, DateTime month)
        {
            return ReadReportForUserInMonth(user, month);
        }

        public void UpdateReportEntry(ReportEntryModel reportEntry)
        {
            throw new NotImplementedException();
        }
    }
}
