using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GameServer.Api
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.MapHttpAttributeRoutes();

      config.Formatters.Remove(config.Formatters.XmlFormatter);
      config.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;

      config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      };
    }
  }
}