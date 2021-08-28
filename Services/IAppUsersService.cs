using System.Linq;
using System.Threading.Tasks;
using webapi.Helpers;
using webapi.Models.app;

namespace webapi.Services
{
  public interface IAppUsersService
  {
    Task<App_users> CreateAsync(App_users param);
    Task<App_users> GetByPhoneNumberAsync(string phoneNumber);
  }

  public class AppUsersService : IAppUsersService
  {
    private ApplicationDbContext _context;

    public AppUsersService(ApplicationDbContext context)
    {
      _context = context;
    }

    public Task<App_users> CreateAsync(App_users param)
    {
      return Task.Run(() =>
      {
        if (string.IsNullOrWhiteSpace(param.phoneNumber))
        {
          throw new AppException("请输入手机号");
        }
        _context.app_users.Add(param);
        _context.SaveChanges();
        return param;
      });
    }

    public Task<App_users> GetByPhoneNumberAsync(string phoneNumber)
    {
      return Task.Run(() =>
      {
        return _context.app_users.FirstOrDefault(x => x.phoneNumber == phoneNumber);
      });
    }
  }
}