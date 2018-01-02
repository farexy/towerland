using System;
using System.Linq;
using System.Web.Http;
using GameServer.Api.Exceptions;

namespace GameServer.Api.Controllers.Base
{
  public class BaseAuthorizeController : ApiController
  {
    protected Guid UserSessionId
    {
      get
      {
        if (!Request.Headers.TryGetValues("user-session", out var sessionId))
        {
          throw new ApiException("No required header found!"); 
        }

        return Guid.Parse(sessionId.SingleOrDefault() ?? throw new ApiException("No required header found!"));
      }
    }
  }
}