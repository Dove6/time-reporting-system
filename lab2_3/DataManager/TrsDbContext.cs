using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Trs.Models.DomainModels;

namespace Trs.DataManager;

public class TrsDbContext : DbContext
{
    public virtual DbSet<AcceptedTime> AcceptedTime { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Project> Projects { get; set; }
    public virtual DbSet<Report> Reports { get; set; }
    public virtual DbSet<ReportEntry> ReportEntries { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public TrsDbContext(DbContextOptions<TrsDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // source: https://stackoverflow.com/questions/52684458/updating-entity-in-ef-core-application-with-sqlite-gives-dbupdateconcurrencyexce
        if (Database.IsSqlite())
        {
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
        }

        modelBuilder.Entity<AcceptedTime>()
            .HasKey(nameof(Models.DomainModels.AcceptedTime.ReportId),
                nameof(Models.DomainModels.AcceptedTime.ProjectCode));

        modelBuilder.Entity<Category>()
            .HasKey(nameof(Category.ProjectCode),
                nameof(Category.Code));

        modelBuilder.Entity<ReportEntry>()
            .HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(nameof(ReportEntry.ProjectCode),
                nameof(ReportEntry.CategoryCode));
    }
}
