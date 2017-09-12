using Microsoft.Practices.Unity;

namespace GameServer.Api
{
  public static class UnityConfig
  {
    public static UnityContainer RegisterComponents()
    {
      var container = new UnityContainer();

      //container.RegisterType<IFieldFactory>()
      // register all your components with the container here
      // it is NOT necessary to register your controllers

      // e.g. container.RegisterType<ITestService, TestService>();

      return container;
    }
  }
}