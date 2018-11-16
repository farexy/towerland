using AutoMapper;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Data.Entities;
using Towerland.GameServer.Models;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Config
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

    private static void CreateMapping(IProfileExpression cfg)
    {
      cfg.CreateMap<StateChangeCommandRequestModel, StateChangeCommand>();
      cfg.CreateMap<LiveBattleModel, ActionsResponseModel>()
        .ForMember(dest => dest.ActionsByTicks, opt => opt.MapFrom(src => src.Ticks))
        .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.State));
      cfg.CreateMap<User, UserDetailsResponseModel>()
        .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Nickname));
    }
  }
}