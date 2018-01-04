using AutoMapper;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Common.Models.State;

namespace Towerland.GameServer.Api
{
  public static class AutoMapperConfiguration
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