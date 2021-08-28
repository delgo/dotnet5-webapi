using Microsoft.EntityFrameworkCore;
using webapi.Models.admin;
using webapi.Models.app;

namespace webapi.Helpers
{
  public class ApplicationDbContext : DbContext
  {
    public DbSet<Admin_users> admin_users { get; set; } //创建实体类添加Context中
    public DbSet<Admin_roles> admin_roles { get; set; }
    public DbSet<App_users> app_users { get; set; }
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

  }
}
