using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WikaApp;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<UserStored> users { get; set; }
  public DbSet<PageStored> pages { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder opts)
  {
    opts.UseSqlite("Data Source=wika.db");
  }

  protected override void OnModelCreating(ModelBuilder model_builder)
  {
    model_builder.Entity<UserStored>().HasKey(x => x.email);
    model_builder.Entity<PageStored>().HasKey(x => x.path);
  }
}

public class UserStored
{
  [MaxLength(256)] public required string email { get; set; }
}

public class PageStored
{
  [MaxLength(256)] public required string         path          { get; set; }
  public required                  string         content       { get; set; }
  public required                  DateTimeOffset last_modified { get; set; }
}