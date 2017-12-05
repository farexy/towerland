using Microsoft.Practices.Unity;
using AutoMapper;
using GameServer.Api.Models;
using GameServer.Common.Models.State;

namespace GameServer.Api
{
  public class AutoMapperConfiguration
  {
    public static IMapper Configure()
    {
      var config = new MapperConfiguration(cfg =>
      {
        cfg.CreateMissingTypeMaps = true;
        cfg.AllowNullCollections = true;
        cfg.AllowNullDestinationValues = true;
        CreateMapping(cfg);
      });

      return config.CreateMapper();
    }

    private static void CreateMapping(IMapperConfiguration cfg)
    {
      cfg.CreateMap<StateChangeCommandRequestModel, StateChangeCommand>();
    }
  }
}