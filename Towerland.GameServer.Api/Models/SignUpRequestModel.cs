namespace GameServer.Api.Models
{
  public class SignUpRequestModel
  {
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } 
    public string Nickname { get; set; }
  }
}