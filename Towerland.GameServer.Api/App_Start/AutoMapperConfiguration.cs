using Microsoft.Practices.Unity;
using AutoMapper;

namespace GameServer.Api
{
  public class AutoMapperConfiguration
  {
    public static void Configure(IUnityContainer container)
    {
      var config = new MapperConfiguration(cfg =>
      {
        cfg.CreateMissingTypeMaps = true;
        cfg.AllowNullCollections = true;
        cfg.AllowNullDestinationValues = true;
        CreateMapping(cfg);
      });

      var mapper = config.CreateMapper();

      container.RegisterInstance(mapper);
    }

    private static void CreateMapping(IMapperConfiguration cfg)
    {

    }
  }
}