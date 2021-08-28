using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using webapi.Controllers.admin;
using webapi.Helpers;
using webapi.Helpers.admin;
using webapi.Services.admin;
using webapi.Models.admin;

namespace webapi
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //cors跨域设置
      services.AddCors(options =>
      {
        options.AddPolicy("policy",
        builder =>
        {
          builder.WithOrigins(Configuration.GetSection("CorsUrl").Value);
        });
      });

      //数据库链接mysql
      string connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
      // Replace "YourDbContext" with the name of your own DbContext derived class.
      services.AddDbContextPool<ApplicationDbContext>(
          dbContextOptions => dbContextOptions
              .UseMySql(
                  // Replace with your connection string.
                  connectionString,
                  // Replace with your server version and type.
                  mySqlOptions => mySqlOptions
                      .ServerVersion(new Version(5, 7, 31), ServerType.MySql)
                      .CharSetBehavior(CharSetBehavior.NeverAppend)
              )
       );

      services.AddControllers();

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "webapi", Version = "v1" });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
           {
              new OpenApiSecurityScheme
              {
                Reference = new OpenApiReference
                  {
                     Id = "Bearer",
                     Type = ReferenceType.SecurityScheme
                  }
              },
              Array.Empty<string>(          )
           }
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
          Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
          Name = "Authorization",//jwt默认的参数名称
          In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
          Type = SecuritySchemeType.ApiKey
        });
      });

      var mappingConfig = new MapperConfiguration(mc =>
        {
          mc.AddProfile(new AutoMapperProfile());
        });
      IMapper mapper = mappingConfig.CreateMapper();
      services.AddSingleton(mapper);
      // configure DI for application services
      services.AddScoped<IUserService, UserService>();
      services.AddScoped<IRolesService, RolesService>();

      services.AddHttpContextAccessor();
      services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

      services.AddAuthorization(options =>
      {
        options.AddPolicy(JwtBearerDefaults.AuthenticationScheme, policy =>
        {
          policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
          policy.RequireClaim(ClaimTypes.Name);
          policy.RequireRole(ClaimTypes.Role);
        });
      });
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
      {
        options.Events = new JwtBearerEvents
        {
          OnTokenValidated = async context =>
          {
            var name = context.Principal.Identity.Name;
            if (context.Principal.IsInRole("App"))
            {
              var appUserService = context.HttpContext.RequestServices.GetRequiredService<webapi.Services.IAppUsersService>();
              var appUser = await appUserService.GetByPhoneNumberAsync(name);
              if (appUser == null)
              {
                context.Fail("App Unauthorized");
              }
            }
            else
            {
              var userService = context.HttpContext.RequestServices.GetRequiredService<webapi.Services.admin.IUserService>();
              var user = await userService.GetByNameAysnc(name);
              if (user == null)
              {
                context.Fail("Admin Unauthorized");
              }
            }
          }
        };
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
              ValidateAudience = false,// 验证秘钥的接受人，如果要验证在这里提供接收人字符串即可
              ValidateIssuer = false,// 验证秘钥发行人，如果要验证在这里指定发行人字符串即可
              ValidateActor = false,
              ValidateLifetime = true,// token有效期
              RequireExpirationTime = false,//是否Claims必须包含Expires过期时间
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("AuthSettings").GetSection("Key").Value))
            };
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "webapi v1"));
      }

      loggerFactory.AddLog4Net(); // << Add this line

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseCors();

      app.UseAuthentication();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
