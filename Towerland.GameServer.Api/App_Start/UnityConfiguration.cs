using System.Configuration;
using AutoMapper;
using GameServer.Common.Models.GameObjects;
using Microsoft.Practices.Unity;
using ServiceStack.Data;
using ServiceStack.OrmLite;
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

      container.RegisterInstance<IDbConnectionFactory>(
        new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["Towerland"].ConnectionString, MySqlDialect.Provider));
      
      container.RegisterType<IProvider<LiveBattleModel>, BattleInMemoryProvider>();
      container.RegisterType<ICrudRepository<Battle>, BattleRepository>();
      container.RegisterType<ICrudRepository<User>, UserRepository>();
      
      container.RegisterType<IBattleService, LiveBattleService>();
      container.RegisterType<IBattleSearchService, BattleSearchService>();
      container.RegisterType<ILiveBattleService, LiveBattleService>();
      container.RegisterType<IUserService, UserService>();

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