using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Trs.Models.DomainModels;

namespace Trs.DataManager;

public class TrsDbContext : DbContext
{
    public const string DbDirectory = "storage";
    private const string DbFilename = "trs.db";
    public static string DbPath => $"{DbDirectory}/{DbFilename}";

    public virtual DbSet<AcceptedTime> AcceptedTime { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<Project> Projects { get; set; } = null!;
    public virtual DbSet<Report> Reports { get; set; } = null!;
    public virtual DbSet<ReportEntry> ReportEntries { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;

    public TrsDbContext(DbContextOptions<TrsDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (Database.IsSqlite())
        {
            // source: https://stackoverflow.com/questions/52684458/updating-entity-in-ef-core-application-with-sqlite-gives-dbupdateconcurrencyexce
            // {
            var timestampProperties = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(byte[])
                            && p.ValueGenerated == ValueGenerated.OnAddOrUpdate
                            && p.IsConcurrencyToken);

            foreach (var property in timestampProperties)
            {
                property.SetDefaultValueSql("randomblob(8)");
            }
            // }

            modelBuilder.Entity<Report>()
                .HasCheckConstraint("CK_Report_Month",
                    "[Month] LIKE '____-__' AND strftime('%s', [Month] || '-01') IS NOT NULL");

            modelBuilder.Entity<ReportEntry>()
                .HasCheckConstraint("CK_ReportEntry_DayOfMonth",
                    "[DayOfMonth] LIKE '__' AND strftime('%s', [ReportMonth] || '-' || [DayOfMonth]) IS NOT NULL");
        }

        modelBuilder.Entity<AcceptedTime>()
            .HasKey(e => new { e.ProjectCode, e.OwnerId, e.ReportMonth });

        modelBuilder.Entity<Category>()
            .HasKey(e => new { e.ProjectCode, e.Code });

        modelBuilder.Entity<Report>()
            .HasKey(e => new { e.OwnerId, e.Month });
    }
}
