namespace webapi.Dtos.admin
{
  public class UsersDto
  {
    public int id { get; set; }
    public string userName { get; set; }
    public string password { get; set; }
    public string phone { get; set; }
    public string userType { get; set; }
    public int roleId { get; set; }
    public string roleKey { get; set; }
    public string name { get; set; }
    public string avatar { get; set; }
    public string email { get; set; }
    public string introduction { get; set; }
    public bool status { get; set; }
    public int createTime { get; set; }
    public int updateTime { get; set; }
  }
}