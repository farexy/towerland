using System.Linq;
using Microsoft.AspNetCore.Http;
using Towerland.GameServer.Exceptions;

namespace Towerland.GameServer.Helpers
{
  public static class RequestExtensions
  {
    public static string GetHeader(this HttpRequest request, string header)
    {
      if (!request.Headers.ContainsKey(header))
      {
        throw new ApiException("No required header found!");
      }

      return request.Headers[header].FirstOrDefault() ?? throw new ApiException("No required header found!");
    }
  }
}