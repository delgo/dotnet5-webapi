using System;
using webapi.Dtos;
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
using webapi.Models;
using webapi.Models.admin;
using webapi.Services.admin;
using webapi.Dtos.admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace webapi.Controllers.admin
{
  [Authorize(Roles = "Admin")]
  [Route("api/[controller]/[action]")]
  [EnableCors("policy")]
  [ApiController]
  public class RolesController : ControllerBase
  {
    private readonly ApplicationDbContext db;
    private IRolesService _rolesService;
    private IMapper _mapper;
    private IActionContextAccessor _accessor;
    private readonly ILogger<RolesController> _logger;
    public RolesController(
          ApplicationDbContext dbContext,
          ILogger<RolesController> logger,
          IRolesService rolesService,
          IMapper mapper,
          IActionContextAccessor httpContextAccessor
          )
    {
      db = dbContext;
      _logger = logger;
      _rolesService = rolesService;
      _mapper = mapper;
      _accessor = httpContextAccessor;
    }

    [HttpPost]
    public async Task<IActionResult> GetRolesList()
    {
      _logger.LogInformation($"GetRolesList");
      var roles = await _rolesService.GetAllAsync();
      return Ok(new
      {
        code = 20000,
        data = roles
      });
    }
    [HttpPost]
    public async Task<IActionResult> AddRole(Admin_roles roleParam)
    {
      _logger.LogInformation($"AddRole {roleParam.roleKey}@{roleParam.name}@{roleParam.description}");

      roleParam.createTime = (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
      roleParam.updateTime = (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
      try
      {
        var _role = await _rolesService.CreateAsync(roleParam);
        return Ok(new
        {
          code = 20000,
          data = _role
        });
      }
      catch (AppException ex)
      {
        _logger.LogWarning($"AddRole AppException" + ex.Message);
        return BadRequest(new { message = ex.Message });
      }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      _logger.LogInformation($"Delete {id}");
      await _rolesService.DeleteAsync(id);
      return Ok(new
      {
        code = 20000,
        data = ""
      });
    }
    [HttpPut]
    public async Task<IActionResult> Update(Admin_roles roleParam)
    {
      _logger.LogInformation($"UpdateRoles {roleParam.id} {roleParam.name}@{roleParam.roleKey}");
      roleParam.updateTime = (Int32)DateTimeOffset.Now.ToUnixTimeSeconds();
      var role = _mapper.Map<Admin_roles>(roleParam);
      try
      {
        var res = await _rolesService.UpdateAsync(role);
        return Ok(
          new
          {
            code = 20000,
            data = res
          });
      }
      catch (AppException ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }
  }
}