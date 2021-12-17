using Trs.Extensions;
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
            new() { Code = "database", ProjectCode = "ARGUS-123" },
            new() { Code = "other", ProjectCode = "ARGUS-123" }
        };
        foreach (var category in categories)
            dbContext.Categories.Add(category);
        dbContext.SaveChanges();

        var reports = new List<Report>
        {
            new() { Id = 1, Month = new DateTime(2021, 11, 1).TrimToMonth(), Frozen = true, OwnerId = 3}
        };
        foreach (var report in reports)
            dbContext.Reports.Add(report);
        dbContext.SaveChanges();

        var reportEntries = new List<ReportEntry>
        {
            new() { Id = 1, Date = new DateTime(2021, 11, 7), Time = 45, Description = "data import", ProjectCode = "ARGUS-123", CategoryCode = "database", ReportId = 1},
            new() { Id = 2, Date = new DateTime(2021, 11, 7), Time = 120, Description = "picie kawy", ProjectCode = "OTHER", ReportId = 1 },
            new() { Id = 3, Date = new DateTime(2021, 11, 8), Time = 45, Description = "kompilacja", ProjectCode = "ARGUS-123", ReportId = 1 },
            new() { Id = 4, Date = new DateTime(2021, 11, 8), Time = 120, Description = "office arrangement", ProjectCode = "OTHER", ReportId = 1 },
            new() { Id = 5, Date = new DateTime(2021, 11, 12), Time = 45, Description = "project meeting", ProjectCode = "ARGUS-123", CategoryCode = "other", ReportId = 1 }
        };
        foreach (var reportEntry in reportEntries)
            dbContext.ReportEntries.Add(reportEntry);
        dbContext.SaveChanges();

        var acceptedTime = new List<AcceptedTime>
        {
            new() { Time = 100, ReportId = 1, ProjectCode = "ARGUS-123" },
            new() { Time = 110, ReportId = 1, ProjectCode = "OTHER" }
        };
        foreach (var acceptedTimeEntry in acceptedTime)
            dbContext.AcceptedTime.Add(acceptedTimeEntry);
        dbContext.SaveChanges();
    }
}
