using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using webapi.Helpers;
using webapi.Models.admin;

namespace webapi.Services.admin
{
  public interface IUserService
  {
    Task<Admin_users> AuthenticateAsync(string username, string password);
    Task<List<Admin_users>> GetAllAsync(int page, int limit);
    Task<int> GetAllTotalAsync();
    Task<Admin_users> GetByIdAsync(int id);
    Task<Admin_users> GetByNameAsync(string name);
    Task<Admin_users> CreateAsync(Admin_users user, string password);
    Task UpdateAsync(Admin_users userParam, string password = null);
    Task DeleteAsync(int id);
    Task<Admin_users> GetByUserNameAsync(string username);
  }

  public class UserService : IUserService
  {
    private ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
      _context = context;
    }

    public Task<Admin_users> AuthenticateAsync(string username, string password)
    {

      return Task.Run(() =>
      {
        Admin_users user = null;
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
          user = _context.admin_users.SingleOrDefault(x => x.userName == username);
        }
        if (user != null && VerifyPasswordHash(password, user.passwordHash, user.passwordSalt))
        {
          return user;
        }
        return null;
      });
    }

    public Task<Admin_users> CreateAsync(Admin_users user, string password)
    {
      return Task.Run(() =>
      {
        if (string.IsNullOrWhiteSpace(password))
        {
          throw new AppException("请输入密码");
        }
        if (_context.admin_users.Any(x => x.userName == user.userName))
        {
          throw new AppException("用户名 \"" + user.userName + "\" 已存在");
        }
        CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
        user.passwordHash = passwordHash;
        user.passwordSalt = passwordSalt;
        _context.admin_users.Add(user);
        _context.SaveChanges();
        return user;
      });
    }

    public Task DeleteAsync(int id)
    {
      return Task.Run(() =>
      {
        var user = _context.admin_users.Find(id);
        if (user != null)
        {
          _context.admin_users.Remove(user);
          _context.SaveChanges();
        }
      });
    }
    public Task<List<Admin_users>> GetAllAsync(int page, int limit)
    {
      return Task<Admin_users>.Run(() =>
      {
        var list = _context.admin_users.Skip(page > 1 ? page * limit - 1 : 0).Take(limit).ToList();
        return list;
      });
    }
    public Task<int> GetAllTotalAsync()
    {
      return Task<Admin_users>.Run(_context.admin_users.Count);
    }
    public Task<Admin_users> GetByIdAsync(int id)
    {
      return Task.Run(() =>
      {
        return _context.admin_users.Find(id);
      });
    }

    public Task<Admin_users> GetByUserNameAsync(string username)
    {
      return Task.Run(() =>
      {
        return _context.admin_users.SingleOrDefault(x => x.userName == username);
      });
    }

    public Task<Admin_users> GetByNameAsync(string name)
    {
      return Task.Run(() =>
      {
        return _context.admin_users.FirstOrDefault(x => x.userName == name);
      });
    }

    public Task UpdateAsync(Admin_users userParam, string password = null)
    {
      return Task.Run(() =>
      {
        var user = _context.admin_users.Find(userParam.id);
        if (user == null)
        {
          throw new AppException("未找到该用户");
        }
        if (userParam.userName != user.userName)
        {
          if (_context.admin_users.Any(x => x.userName == userParam.userName))
          {
            throw new AppException("用户名 " + userParam.userName + " 已存在");
          }
          user.userName = userParam.userName;
        }
        user.phone = userParam.phone;
        user.roleId = userParam.roleId;
        user.roleKey = userParam.roleKey;
        user.name = userParam.name;
        user.avatar = userParam.avatar;
        user.email = userParam.email;
        user.introduction = userParam.introduction;
        user.updateTime = userParam.updateTime;
        user.status = userParam.status;
        if (!string.IsNullOrWhiteSpace(password))
        {
          CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
          user.passwordHash = passwordHash;
          user.passwordSalt = passwordSalt;
        }
        _context.admin_users.Update(user);
        _context.SaveChanges();
      });
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      if (password == null)
      {
        throw new ArgumentNullException(nameof(password));
      }

      if (string.IsNullOrWhiteSpace(password))
      {
        throw new ArgumentException("密码不能有空格", nameof(password));
      }

      if (password.Length < 6)
      {
        throw new ArgumentException("密码长度最少为6位", nameof(password));
      }

      using var hmac = new HMACSHA512();
      passwordSalt = hmac.Key;
      passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
      if (password == null)
      {
        throw new ArgumentNullException(nameof(password));
      }
      if (string.IsNullOrWhiteSpace(password))
      {
        throw new ArgumentException("密码不能用空格", nameof(password));
      }
      if (storedHash.Length != 64)
      {
        throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));
      }
      if (storedSalt.Length != 128)
      {
        throw new ArgumentException("Invalid length of password salt (128 bytes expected).", nameof(storedSalt));
      }
      if (password.Length < 6)
      {
        throw new ArgumentException("密码最少为6位", nameof(password));
      }
      using (var hmac = new HMACSHA512(storedSalt))
      {
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        for (int i = 0; i < computedHash.Length; i++)
        {
          if (computedHash[i] != storedHash[i])
          {
            return false;
          }
        }
      }
      return true;
    }
  }
}