using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Towerland.GameServer.Helpers;

namespace Towerland.GameServer.Controllers
{
  public class BaseAuthorizeController : ControllerBase
  {
    protected Task<Guid> UserSessionIdAsync => UserSessionHelper.GetUserIdAsync(Request.GetHeader("user-session"));
  }
}