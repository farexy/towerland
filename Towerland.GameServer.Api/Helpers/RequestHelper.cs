using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GameServer.Api.Helpers
{
  public class RequestHelper
  {
    public static async Task<T> ReadBody<T>(HttpRequestMessage request)
    {
      await request.Content.LoadIntoBufferAsync();
      var paymentRequest = await request.Content.ReadAsAsync<T>();
      var requestBody = await request.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<T>(requestBody);
    } 
  }
}