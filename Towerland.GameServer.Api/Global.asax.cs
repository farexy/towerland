using System;
using System.Web.Http;
using Unity.WebApi;

namespace GameServer.Api
{
  public class Global : System.Web.HttpApplication
  {
    protected void Application_Start(object sender, EventArgs e)
    {
      var container = UnityConfig.RegisterComponents();
      GlobalConfiguration.Configure(c => WebApiConfig.Register(c, container));
      GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
      AutoMapperConfiguration.Configure(container);
    }
  }
}