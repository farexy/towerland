using System.Configuration;
using AutoMapper;
using Microsoft.Practices.Unity;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Logic.SpecialAI;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Domain.Infrastructure;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Api
{
  public static class UnityConfig
  {
    public static UnityContainer RegisterComponents()
    {
      var container = new UnityContainer();

      container.RegisterInstance<IDbConnectionFactory>(
        new OrmLiteConnectionFactory(ConfigurationManager.ConnectionStrings["Towerland"].ConnectionString, MySqlDialect.Provider));

      container.RegisterType<IProvider<LiveBattleModel>, BattleInMemoryProvider>();
      
      //repos
      container.RegisterType<IBattleRepository, BattleRepository>();
      container.RegisterType<IUserRepository, UserRepository>();
//      container.RegisterType<IBattleRepository, FakeBattleRepository>();
//      container.RegisterType<IUserRepository, FakeUserRepository>();
      
      container.RegisterType<IBattleInitializationService, LiveBattleService>();
      container.RegisterType<IBattleSearchService, BattleSearchService>();
      container.RegisterType<ILiveBattleService, LiveBattleService>();
      container.RegisterType<IUserService, UserService>();

      container.RegisterType<ICheatCommandManager, CheatCommandManager>();
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