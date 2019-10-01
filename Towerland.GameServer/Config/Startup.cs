using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using Towerland.GameServer.AI;
using Towerland.GameServer.BusinessLogic.Infrastructure;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Data.DataAccess;
using Towerland.GameServer.Logic;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.Selectors;
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
      services.AddMvc(opt => opt.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

      services.AddSingleton(AutoMapperConfiguration.Configure());

      MySqlDialect.Provider.StringSerializer = new JsonStringSerializer();
      services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(Configuration.GetConnectionString("Towerland"), MySqlDialect.Provider));
      services.AddSingleton<IProvider<LiveBattleModel>, BattleInMemoryProvider>();

      //services.AddTransient<IBattleRepository, BattleRepository>();
      //services.AddTransient<IUserRepository, UserRepository>();
      services.AddTransient<IBattleRepository, FakeBattleRepository>();
      services.AddTransient<IUserRepository, FakeUserRepository>();

      services.AddScoped<IUserService, UserService>();

      services.AddSingleton<IBattleSearchService, BattleSearchService>();

      services.AddSingleton<LiveBattleService>();
      services.AddSingleton<IBattleInitializationService>(x => x.GetRequiredService<LiveBattleService>());
      services.AddSingleton<ILiveBattleService>(x => x.GetRequiredService<LiveBattleService>());

      services.AddHostedService<BattleAiService>();

      services.AddSingleton<IStatsLibrary, StatsLibrary>();
      services.AddTransient<ICheatCommandManager, CheatCommandManager>();
      services.AddTransient<IStatsProvider, StatsFactory>();
      services.AddTransient<ICheatCommandManager, CheatCommandManager>();
      services.AddTransient<IFieldStorage, FieldStorageStub>();
      services.AddTransient<IPathChooser, PathChooser>();
      services.AddTransient<IStateChangeRecalculator, StateChangeRecalculator>();
      services.AddTransient<IGameObjectFactory<Unit>, UnitFactory>();
      services.AddTransient<IGameObjectFactory<Tower>, TowerFactory>();
      services.AddTransient<IUnitSelector, UnitSelector>();
      services.AddTransient<ITowerSelector, TowerSelector>();
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
        //app.UseHsts();
      }

      app.UseMiddleware<ErrorHandlingMiddleware>();
      app.UseMvc();
    }
  }
}