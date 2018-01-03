using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GameServer.Api.Exceptions;
using Newtonsoft.Json;

namespace GameServer.Api.Helpers
{
  public static class RequestExtensions
  {
    public static async Task<T> ReadBody<T>(this HttpRequestMessage request)
    {
      await request.Content.LoadIntoBufferAsync();
      var requestBody = await request.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<T>(requestBody);
    }

    public static string GetHeader(this HttpRequestMessage request, string header)
    {
      if (!request.Headers.TryGetValues(header, out var values))
      {
        throw new ApiException("No required header found!"); 
      }

      return values.SingleOrDefault() ?? throw new ApiException("No required header found!");
    }
  }
}