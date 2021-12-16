using Microsoft.EntityFrameworkCore;
using Trs.Models.DbModels;

namespace TRS.DataManager;

public class TrsDbContext : DbContext
{
    public TrsDbContext(DbContextOptions<TrsDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
}
