using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using webapi.Helpers;
using webapi.Models.admin;
using webapi.Services.admin;
using Newtonsoft.Json;
using webapi.Dtos.admin;

namespace webapi.Controllers.admin
{
  [Authorize(Roles = "Admin")]
  [Route("api/[controller]/[action]")]
  [EnableCors("policy")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    public static JwtSecurityTokenHandler JwtTokenHandler = new JwtSecurityTokenHandler();
    private readonly ApplicationDbContext db;
    private IUserService _userService;
    private IRolesService _rolesService;
    private IMapper _mapper;
    private IActionContextAccessor _accessor;
    private readonly ILogger<UsersController> _logger;
    private IConfiguration _configuration;
    public UsersController(
      ApplicationDbContext dbContext,
      ILogger<UsersController> logger,
      IUserService userService,
      IRolesService rolesService,
      IMapper mapper,
      IActionContextAccessor httpContextAccessor,
      IConfiguration configuration)
    {
      db = dbContext;
      _logger = logger;
      _userService = userService;
      _rolesService = rolesService;
      _mapper = mapper;
      _accessor = httpContextAccessor;
      _configuration = configuration;
    }

    /// <summary>
    /// 用户验证
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]

    public async Task<IActionResult> Authenticate(UsersDto userDto)
    {
      _logger.LogInformation($"Authenticate {userDto.userName}@{userDto.password}");

      var user = await _userService.AuthenticateAsync(userDto.userName, userDto.password);

      if (user == null) return BadRequest(new { Message = "用户名或密码错误" });
      if (!user.status) return BadRequest(new { Message = "该用户被禁用，请联系管理员" });

      var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AuthSettings").GetSection("Key").Value));
      var claims = new[] { new Claim(ClaimTypes.Name, user.userName), new Claim(ClaimTypes.Role, user.userType) };
      var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
      var token = new JwtSecurityToken("WebAPIServer", "WebAPIClients", claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);
      var tokenString = JwtTokenHandler.WriteToken(token);

      var returnData = new
      {
        tokenType = "Bearer",
        accessToken = tokenString
      };
      return Ok(new
      {
        code = 20000,
        data = returnData
      });
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> getUserInfo()
    {
      var username = Response.HttpContext.User.Identity.Name;//获取当前登陆的用户名对应new Claim(ClaimTypes.Name, user.UserName)
      _logger.LogInformation($"GetByUserName {username}");
      var user = await _userService.GetByUserNameAsync(username);
      var userDto = _mapper.Map<UsersDto>(user);
      _logger.LogInformation($"GetRolesByIdAsync {userDto.id}");
      var roles = await _rolesService.GetByIdAsync(userDto.roleId);

      var jsonRoutes = JsonConvert.DeserializeObject<Routes[]>(roles.routes);
      var jsonRoleKey = roles.roleKey;

      var returnData = new
      {
        asyncRoutes = jsonRoutes,
        roleKey = jsonRoleKey,
        avatar = userDto.avatar,
        name = userDto.name,
        introduction = userDto.introduction,
        email = userDto.email
      };

      return Ok(new
      {
        code = 20000,
        data = returnData
      });
    }
    /// <summary>
    /// 添加用户
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> addUser(UsersDto userDto)
    {
      _logger.LogInformation($"Register {userDto.userName}@{userDto.password}@{userDto.phone}");
      var user = _mapper.Map<Admin_users>(userDto);
      try
      {
        user.userType = "Admin";
        user.createTime = (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
        user.updateTime = (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
        var _user = await _userService.CreateAsync(user, userDto.password);
        return Ok(new
        {
          code = 20000,
          data = _mapper.Map<UsersDto>(_user)
        });
      }
      catch (AppException ex)
      {
        _logger.LogWarning($"Register AppException" + ex.Message);
        return BadRequest(new { message = ex.Message });
      }
    }
    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> updateUser(UsersDto userDto)
    {
      _logger.LogInformation($"Update {userDto.id} {userDto.userName}@{userDto.password}@{userDto.phone}");
      var user = _mapper.Map<Admin_users>(userDto);
      user.id = userDto.id;
      try
      {
        await _userService.UpdateAsync(user, userDto.password);
        return Ok(new
        {
          code = 20000,
          data = user
        });
      }
      catch (AppException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("{page}/{limit}")]
    public async Task<IActionResult> getUsersList(int page, int limit)
    {
      _logger.LogInformation($"GetAll");
      var users = await _userService.GetAllAsync(page, limit);
      var userDtos = _mapper.Map<List<UsersDto>>(users);
      var total = await _userService.GetAllTotalAsync();
      return Ok(new
      {
        code = 20000,
        data = new { list = userDtos, total = total }
      });
    }
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
      _logger.LogInformation($"GetById {id}");
      var user = await _userService.GetByIdAsync(id);
      var userDto = _mapper.Map<UsersDto>(user);
      return Ok(userDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UsersDto userDto)
    {
      _logger.LogInformation($"Update {id} {userDto.userName}@{userDto.password}@{userDto.phone}");
      var user = _mapper.Map<Admin_users>(userDto);
      user.id = id;
      try
      {
        await _userService.UpdateAsync(user, userDto.password);
        return Ok();
      }
      catch (AppException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> deleteUser(int id)
    {
      _logger.LogInformation($"Delete {id}");
      await _userService.DeleteAsync(id);
      return Ok(new
      {
        code = 20000,
        data = ""
      });
    }
  }
}