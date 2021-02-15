using AutoMapper;
using webapi.Dtos.admin;
using webapi.Models.admin;

namespace webapi.Helpers.admin
{
  public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
      CreateMap<Admin_users, UsersDto>();
      CreateMap<UsersDto, Admin_users>();
    }
  }
}