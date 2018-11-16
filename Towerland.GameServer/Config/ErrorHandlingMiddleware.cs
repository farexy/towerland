using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Towerland.GameServer.Exceptions;
using Towerland.GameServer.Models.Exceptions;

namespace Towerland.GameServer.Config
{
  public class ErrorHandlingMiddleware
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      try
      {
        await _next.Invoke(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      var message = exception.GetBaseException().Message;
      var statusCode = HttpStatusCode.InternalServerError;
      if (exception is LogicException)
      {
        statusCode = HttpStatusCode.BadRequest;
        message = exception.Message;
      }
      else if (exception is ApiException)
      {
        statusCode = HttpStatusCode.Forbidden;
        message = exception.Message;
        Logger.Warn(message);
      }
      else
      {
        Logger.Error("Unknown exception", exception);
      }

      var result = JsonConvert.SerializeObject(new {message});
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)statusCode;
      await context.Response.WriteAsync(result);
    }
  }
}