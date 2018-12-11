using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Towerland.GameServer.BusinessLogic.Infrastructure;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Data.DataAccess;
using Towerland.GameServer.Logic;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.SpecialAI;
using Towerland.GameServer.Models.GameObjects;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Towerland.GameServer.Config
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      services.AddSingleton(AutoMapperConfiguration.Configure());

      services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(Configuration.GetConnectionString("Towerland"), MySqlDialect.Provider));
      services.AddSingleton<IProvider<LiveBattleModel>, BattleInMemoryProvider>();
//
//      services.AddScoped<IBattleRepository, BattleRepository>();
//      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IBattleRepository, FakeBattleRepository>();
      services.AddScoped<IUserRepository, FakeUserRepository>();

      services.AddScoped<IUserService, UserService>();

      services.AddSingleton<IBattleSearchService, BattleSearchService>();

      services.AddSingleton<LiveBattleService>();
      services.AddSingleton<IBattleInitializationService>(x => x.GetRequiredService<LiveBattleService>());
      services.AddSingleton<ILiveBattleService>(x => x.GetRequiredService<LiveBattleService>());

      services.AddSingleton<IStatsLibrary, StatsLibrary>();
      services.AddScoped<ICheatCommandManager, CheatCommandManager>();
      services.AddScoped<IStatsProvider, StatsFactory>();
      services.AddScoped<ICheatCommandManager, CheatCommandManager>();
      services.AddScoped<IFieldFactory, FieldFactoryStub>();
      services.AddScoped<IPathChooser, PathChooser>();
      services.AddScoped<IStateChangeRecalculator, StateChangeRecalculator>();
      services.AddScoped<IGameObjectFactory<Unit>, UnitFactory>();
      services.AddScoped<IGameObjectFactory<Tower>, TowerFactory>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseMiddleware<ErrorHandlingMiddleware>();
      app.UseMvc();
    }
  }
}