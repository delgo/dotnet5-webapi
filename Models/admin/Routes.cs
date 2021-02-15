namespace webapi.Models.admin
{
  public class Routes
  {
    public Meta meta { get; set; }
    public string name { get; set; }
    public string path { get; set; }
    public string redirect { get; set; }
    public string component { get; set; }
    public Children[] children { get; set; }
  }
  public class Meta
  {
    public string icon { get; set; }
    public string[] roles { get; set; }
    public string title { get; set; }
    public string affix { get; set; }
  }

  public class Children
  {
    public Meta meta { get; set; }
    public string name { get; set; }
    public string path { get; set; }
    public string redirect { get; set; }
    public string component { get; set; }
    public Children[] children { get; set; }
  }
}