using System;
using System.Web.Http;
using GameServer.Api.Helpers;

namespace GameServer.Api.Controllers.Base
{
  public class BaseAuthorizeController : ApiController
  {
    protected Guid UserSessionId => Guid.Parse(Request.GetHeader("user-session"));
  }
}