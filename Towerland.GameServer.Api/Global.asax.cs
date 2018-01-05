using System;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Http;
using log4net;
using log4net.Config;
using Unity.WebApi;

namespace Towerland.GameServer.Api
{
  public class Global : System.Web.HttpApplication
  {    
    private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    protected void Application_Start(object sender, EventArgs e)
    {
      XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + @"log.config"));

      Log.Info("Game Server is starting");
      
      var container = UnityConfig.RegisterComponents();
      GlobalConfiguration.Configure(c => WebApiConfig.Register(c, container));
      GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
      
      Log.Info("Game Server tool is started");
    }
    
    protected void Application_End(object sender, EventArgs e)
    {
      Log.InfoFormat("Configuration tool is stopped. Reason {0}", HostingEnvironment.ShutdownReason);
    }
    
    protected void Application_Error(object sender, EventArgs e)
    {
      var ex = Server.GetLastError();
      Log.Fatal("Unhandled exception", ex);
      Server.ClearError();
    }
  }
}