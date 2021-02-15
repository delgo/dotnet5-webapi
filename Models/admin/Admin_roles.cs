using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models.admin
{
  public class Admin_roles
  {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int id { get; set; }

    [Required, MaxLength(20)]
    public string roleKey { get; set; }

    [Required, MaxLength(20)]
    public string name { get; set; }

    public string description { get; set; }

    [Required]
    public string routes { get; set; }

    [Required]
    public int createTime { get; set; }

    [Required]
    public int updateTime { get; set; }
  }
}