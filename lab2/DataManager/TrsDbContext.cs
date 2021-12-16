using Microsoft.EntityFrameworkCore;
using Trs.Models.DbModels;

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
        modelBuilder.Entity<AcceptedTime>()
            .HasKey(nameof(Trs.Models.DbModels.AcceptedTime.ReportId), nameof(Trs.Models.DbModels.AcceptedTime.ProjectCode));
    }
}
