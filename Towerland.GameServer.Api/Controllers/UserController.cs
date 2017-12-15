using System.Web.Http;
using Towerland.GameServer.Domain.Interfaces;

namespace GameServer.Api.Controllers
{
  public class UserController : ApiController
  {
    private readonly IUserService _userService;

    public UserController(IUserService service)
    {
      _userService = service;
    }
  }
}