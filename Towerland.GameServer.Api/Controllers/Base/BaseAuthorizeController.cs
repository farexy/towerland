using System;
using System.Web.Http;
using Towerland.GameServer.Api.Helpers;

namespace Towerland.GameServer.Api.Controllers.Base
{
  public class BaseAuthorizeController : ApiController
  {
    protected Guid UserSessionId => UserSessionHelper.GetUserId(Request.GetHeader("user-session"));
  }
}