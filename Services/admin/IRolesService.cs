using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using webapi.Helpers;
using webapi.Models.admin;
using Microsoft.Extensions.Configuration;

namespace webapi.Services.admin
{
  public interface IRolesService
  {
    Task<Admin_roles> GetByIdAysnc(int id);
    Task<List<Admin_roles>> GetAllAysnc();
    Task<Admin_roles> CreateAysnc(Admin_roles role);
    Task DeleteAysnc(int id);
    Task<Admin_roles> UpdateAysnc(Admin_roles roleParam);
  }

  public class RolesService : IRolesService
  {
    private ApplicationDbContext _context;
    private IConfiguration _configuration;
    public RolesService(ApplicationDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }
    public Task<Admin_roles> GetByIdAysnc(int id)
    {
      return Task.Run(() =>
      {
        return _context.admin_roles.Find(id);
      });
    }
    public Task<List<Admin_roles>> GetAllAysnc()
    {
      return Task.Run(_context.admin_roles.ToList);
    }
    // public Task<Routes[]> GetBaseRoutes()
    // {
    //   return Task<Routes[]>.Run(() =>
    //   {
    //     return _configuration.GetSection("AdminRouter").Get<Routes[]>();
    //   });
    // }
    public Task<Admin_roles> CreateAysnc(Admin_roles role)
    {
      return Task.Run(() =>
      {
        if (_context.admin_roles.Any(x => x.roleKey == role.roleKey))
        {
          throw new AppException("该Key \"" + role.roleKey + "\" 已存在");
        }
        if (_context.admin_roles.Any(x => x.name == role.name))
        {
          throw new AppException("该角色 \"" + role.name + "\" 已存在");
        }
        _context.admin_roles.Add(role);
        _context.SaveChanges();
        return role;
      });
    }

    public Task DeleteAysnc(int id)
    {
      return Task.Run(() =>
      {
        var role = _context.admin_roles.Find(id);
        if (role != null)
        {
          _context.admin_roles.Remove(role);
          _context.SaveChanges();
        }
      });
    }

    public Task<Admin_roles> UpdateAysnc(Admin_roles roleParam)
    {
      return Task.Run(() =>
      {
        var role = _context.admin_roles.Find(roleParam.id);
        if (role == null)
        {
          throw new AppException("未找到该角色");
        }
        role.name = roleParam.name;
        role.roleKey = roleParam.roleKey;
        role.description = roleParam.description;
        role.routes = roleParam.routes;
        role.updateTime = roleParam.updateTime;

        _context.admin_roles.Update(role);
        _context.SaveChanges();
        return role;
      });
    }
  }
}