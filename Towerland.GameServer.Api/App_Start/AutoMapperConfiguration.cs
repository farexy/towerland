using AutoMapper;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Common.Models.State;
using Towerland.GameServer.Domain.Models;

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
      cfg.CreateMap<LiveBattleModel, ActionsResponseModel>()
        .ForMember(dest => dest.ActionsByTicks, opt => opt.MapFrom(src => src.Ticks))
        .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.State));
    }
  }
}