using Microsoft.AspNetCore.Mvc;

namespace Towerland.GameServer.Controllers
{
  [Route("")]
  public class HomeController : ControllerBase
  {
    [HttpGet("")]
    public string Index()
    {
      return "Towerland is running";
    }
  }
}