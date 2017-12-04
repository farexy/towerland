using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using AutoMapper;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unity.WebApi;

namespace GameServer.Api
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config, IUnityContainer container)
    {
      config.MapHttpAttributeRoutes();
      config.DependencyResolver = new UnityDependencyResolver(container);

      config.Formatters.Remove(config.Formatters.XmlFormatter);
      config.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;

      config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new JsonContractResolver(new JsonMediaTypeFormatter())
      };
      
      config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
    }
  }
}