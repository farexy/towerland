using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Towerland.GameServer.Config;

namespace Towerland.GameServer
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>()
        .UseUrls("http://0.0.0.0:64283")
        .ConfigureLogging((hostingContext, logging) => logging.AddLog4Net())
        .CaptureStartupErrors(true);
  }
}