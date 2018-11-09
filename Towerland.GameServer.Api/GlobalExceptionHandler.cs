using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using log4net;
using Towerland.GameServer.Api.Exceptions;
using Towerland.GameServer.Common.Models.Exceptions;

namespace Towerland.GameServer.Api
{
  public class GlobalExceptionHandler : ExceptionHandler
  {
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public override void Handle(ExceptionHandlerContext context)
    {
      Exception ex = context.Exception;
      string message = ex.Message;
      HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
      if (ex is LogicException)
      {
        statusCode = HttpStatusCode.BadRequest;
        Log.WarnFormat(CultureInfo.InvariantCulture, "LogicException occured: {0}", ex.Message);
      }
      else if (ex is ApiException)
      {
        statusCode = HttpStatusCode.Forbidden;
        Log.WarnFormat(CultureInfo.InvariantCulture, "ApiException occured: {0}", ex.Message);
      }
      else
      {
        Log.Error("Unsupported exception", context.Exception);
      }

      context.Result = new ErrorResult
      {
        StatusCode = statusCode,
        Request = context.ExceptionContext.Request,
        Error = new ErrorResponseModel {Message = message}
      };
    }

    public override bool ShouldHandle(ExceptionHandlerContext context)
    {
      return true;
    }

    private class ErrorResult : IHttpActionResult
    {
      public HttpRequestMessage Request { get; set; }
      public ErrorResponseModel Error { get; set; }
      public HttpStatusCode StatusCode { get; set; }

      public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
      {
        var response =
          new HttpResponseMessage(StatusCode)
          {
            Content = new ObjectContent<ErrorResponseModel>(Error, GlobalConfiguration.Configuration.Formatters.JsonFormatter),
            RequestMessage = Request
          };
        return Task.FromResult(response);
      }
    }

    private class ErrorResponseModel
    {
      public int Code { get; set; } // For future using
      public string Message { get; set; }
      public string Description { get; set; } // For future using
    }
  }
}