using Trs.Models.DomainModels;

namespace Trs.DataManager;

public static class TrsDbInitializer
{
    public static void Initialize(TrsDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();

        if (dbContext.Categories.Any() || dbContext.Projects.Any() || dbContext.Reports.Any() ||
            dbContext.Users.Any() || dbContext.AcceptedTime.Any() || dbContext.ReportEntries.Any())
        {
            return; // DB already initialized
        }

        var users = new List<User>
        {
            new() { Id = 1, Name = "nowak" },
            new() { Id = 2, Name = "smith" },
            new() { Id = 3, Name = "kowalski" }
        };
        foreach (var user in users)
            dbContext.Users.Add(user);
        dbContext.SaveChanges();

        var projects = new List<Project>
        {
            new() { Code = "ARGUS-123", Name = "Argus", Budget = 125, Active = true, ManagerId = 1 },
            new() { Code = "OTHER", Name = "Other", Budget = -1, Active = true, ManagerId = 2 }
        };
        foreach (var project in projects)
            dbContext.Projects.Add(project);
        dbContext.SaveChanges();

        var categories = new List<Category>
        {
            new() { Code = "", ProjectCode = "ARGUS-123" },
            new() { Code = "database", ProjectCode = "ARGUS-123" },
            new() { Code = "other", ProjectCode = "ARGUS-123" },
            new() { Code = "", ProjectCode = "OTHER" }
        };
        foreach (var category in categories)
            dbContext.Categories.Add(category);
        dbContext.SaveChanges();

        var reports = new List<Report>
        {
            new() { OwnerId = 3, Month = "2021-11", Frozen = true }
        };
        foreach (var report in reports)
            dbContext.Reports.Add(report);
        dbContext.SaveChanges();

        var reportEntries = new List<ReportEntry>
        {
            new() { Id = 1, DayOfMonth = "07", Time = 45, Description = "data import", ProjectCode = "ARGUS-123", CategoryCode = "database", OwnerId = 3, ReportMonth = "2021-11" },
            new() { Id = 2, DayOfMonth = "07", Time = 120, Description = "picie kawy", ProjectCode = "OTHER", OwnerId = 3, ReportMonth = "2021-11" },
            new() { Id = 3, DayOfMonth = "08", Time = 45, Description = "kompilacja", ProjectCode = "ARGUS-123", OwnerId = 3, ReportMonth = "2021-11" },
            new() { Id = 4, DayOfMonth = "08", Time = 120, Description = "office arrangement", ProjectCode = "OTHER", OwnerId = 3, ReportMonth = "2021-11" },
            new() { Id = 5, DayOfMonth = "12", Time = 45, Description = "project meeting", ProjectCode = "ARGUS-123", CategoryCode = "other", OwnerId = 3, ReportMonth = "2021-11" }
        };
        foreach (var reportEntry in reportEntries)
            dbContext.ReportEntries.Add(reportEntry);
        dbContext.SaveChanges();

        var acceptedTime = new List<AcceptedTime>
        {
            new() { Time = 100, OwnerId = 3, ReportMonth = "2021-11", ProjectCode = "ARGUS-123" },
            new() { Time = 110, OwnerId = 3, ReportMonth = "2021-11", ProjectCode = "OTHER" }
        };
        foreach (var acceptedTimeEntry in acceptedTime)
            dbContext.AcceptedTime.Add(acceptedTimeEntry);
        dbContext.SaveChanges();
    }
}
