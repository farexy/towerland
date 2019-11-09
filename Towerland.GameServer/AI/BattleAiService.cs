using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Hosting;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.AI
{
  public abstract class BattleAiService : BackgroundService
  {
    protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);
    private DateTime _lastRun;

    private readonly IProvider<LiveBattleModel> _liveBattleProvider;
    private readonly ILiveBattleService _liveBattleService;
    private readonly IAnalyticsService _analyticsService;
    private readonly PlayerSide _playerSide;
    private readonly Process _process;
    protected readonly Stopwatch Stopwatch;

    protected BattleAiService(
      IProvider<LiveBattleModel> liveBattleProvider,
      ILiveBattleService liveBattleService,
      IAnalyticsService analyticsService,
      PlayerSide playerSide)
    {
      _liveBattleProvider = liveBattleProvider;
      _liveBattleService = liveBattleService;
      _analyticsService = analyticsService;
      _playerSide = playerSide;
      _process = Process.GetCurrentProcess();
      Stopwatch = new Stopwatch();
    }

    protected long CurrentMemoryUsageKb => _process.PrivateMemorySize64 / 1024;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          if (DateTime.Now - Interval > _lastRun)
          {
            await RunSessionAsync(stoppingToken);
            _lastRun = DateTime.Now;
          }

          await Task.Delay(DateTime.Now - _lastRun > Interval ? Interval : DateTime.Now - _lastRun, stoppingToken);
        }
        catch (Exception e)
        {
          Logger.Fatal("Error in background AI worker", e);
        }
      }
    }

    private async Task RunSessionAsync(CancellationToken stoppingToken)
    {
      await Task.WhenAll(
        _liveBattleProvider.GetAll()
          .Where(b => b.CompPlayerSide == _playerSide || b.CompPlayerSide is PlayerSide.Both)
          .Select(async battle =>
          {
            var battleCopy = battle.CreateCopy();
            _liveBattleService.ResolveActions(battleCopy);

            var cmd = await GetCmdAsync(battle, battleCopy, stoppingToken);
            await Task.WhenAll(_analyticsService.LogAsync(cmd), _liveBattleService.RecalculateAsync(cmd));
          }).ToArray());
    }

    protected abstract Task<StateChangeCommand> GetCmdAsync(LiveBattleModel battle, LiveBattleModel battleCopy, CancellationToken stoppingToken);
  }
}