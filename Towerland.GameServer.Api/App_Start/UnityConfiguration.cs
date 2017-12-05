using AutoMapper;
using GameServer.Common.Models.GameObjects;
using Microsoft.Practices.Unity;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Logic.SpecialAI;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Infrastructure;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace GameServer.Api
{
  public static class UnityConfig
  {
    public static UnityContainer RegisterComponents()
    {
      var container = new UnityContainer();

      container.RegisterType<IProvider<LiveBattleModel>, BattleInMemoryProvider>();
      container.RegisterType<ICrudRepository<Battle>, MySqlCrudRepository<Battle>>();
      
      container.RegisterType<IBattleService, LiveBattleService>();
      container.RegisterType<IBattleSearchService, BattleSearchService>();
      container.RegisterType<ILiveBattleService, LiveBattleService>();

      container.RegisterType<IStatsLibrary, StatsLibrary>();
      container.RegisterType<IFieldFactory, FieldFactoryStub>();
      container.RegisterType<IPathOptimiser, PathOptimisation>();
      container.RegisterType<IStateChangeRecalculator, StateChangeRecalculator>();
      container.RegisterType<IGameObjectFactory<Unit>, UnitFactory>();
      container.RegisterType<IGameObjectFactory<Tower>, TowerFactory>();
      
      container.RegisterType<IMapper>(new ContainerControlledLifetimeManager(),
        new InjectionFactory(c => AutoMapperConfiguration.Configure()));
      
      return container;
    }
  }
}