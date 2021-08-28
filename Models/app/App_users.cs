using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models.app
{
  public class App_users
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [Required]
    public string userType { get; set; }
    public string avatar { get; set; }
    public string realName { get; set; }

    [Required, MinLength(11)]
    public string phoneNumber { get; set; }
    public int createTime { get; set; }
    public int updateTime { get; set; }
  }
}