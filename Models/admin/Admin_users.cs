using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models.admin
{
  public class Admin_users
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [Required, MaxLength(64)]
    public string userName { get; set; }

    [Required, MaxLength(128)]
    public byte[] passwordHash { get; set; }

    [Required, MaxLength(128)]
    public byte[] passwordSalt { get; set; }

    [Required]
    public int roleId { get; set; }

    [Required, MaxLength(20)]
    public string roleKey { get; set; }

    [MaxLength(20)]
    public string name { get; set; }

    [MaxLength(200)]
    public string avatar { get; set; }

    [MaxLength(200)]
    public string email { get; set; }

    public string introduction { get; set; }

    [MaxLength(20)]
    public string phone { get; set; }

    [Required, MaxLength(20)]
    public string userType { get; set; }

    [Required]
    public bool status { get; set; }
    public int createTime { get; set; }

    public int updateTime { get; set; }
  }
}