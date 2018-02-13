using System;
using System.Threading.Tasks;
using System.Web.Http;
using Towerland.GameServer.Api.Helpers;

namespace Towerland.GameServer.Api.Controllers.Base
{
  public class BaseAuthorizeController : ApiController
  {
    protected Task<Guid> UserSessionIdAsync => UserSessionHelper.GetUserIdAsync(Request.GetHeader("user-session"));
  }
}