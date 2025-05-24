using Microsoft.EntityFrameworkCore;

namespace WikaApp;

// todo: purpose is unclear
public class PageService(AppDbContext dbContext)
{
  private readonly AppDbContext _db_context = dbContext;
}