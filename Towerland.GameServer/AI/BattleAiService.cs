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
  public class BattleAiService : BackgroundService, IBattleAiService
  {
    private static readonly ILog LoggerAI = LogManager.GetLogger(Assembly.GetEntryAssembly(), "AI");
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(1);
    private DateTime _lastRun;

    private readonly IProvider<LiveBattleModel> _liveBattleProvider;
    private readonly ILiveBattleService _liveBattleService;
    private readonly IUnitSelector _unitSelector;
    private readonly ITowerSelector _towerSelector;
    private readonly IAnalyticsService _analyticsService;
    private readonly Stopwatch _stopwatch;

    public BattleAiService(IProvider<LiveBattleModel> liveBattleProvider, ILiveBattleService liveBattleService,
      IUnitSelector unitSelector, ITowerSelector towerSelector, IAnalyticsService analyticsService)
    {
      _liveBattleProvider = liveBattleProvider;
      _liveBattleService = liveBattleService;
      _unitSelector = unitSelector;
      _towerSelector = towerSelector;
      _analyticsService = analyticsService;
      _stopwatch = new Stopwatch();
    }

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

    public async Task RunSessionAsync(CancellationToken stoppingToken)
    {
      await Task.WhenAll(_liveBattleProvider.GetAll().Select(async battle =>
      {
        var battleCopy = battle.CreateCopy();
        _liveBattleService.ResolveActions(battleCopy);

        LoggerAI.Info("Process for adding unit started");
        _stopwatch.Restart();
        var unitToAdd = await Task.Run(() => _unitSelector.GetNewUnit(battleCopy.State), stoppingToken);
        _stopwatch.Stop();
        LoggerAI.Info($"Process for adding unit finished in {_stopwatch.ElapsedMilliseconds} ms");

        LoggerAI.Info("Process for adding tower started");
        _stopwatch.Restart();
        var towerToAdd = await Task.Run(() => _towerSelector.GetNewTower(battleCopy.State), stoppingToken);
        _stopwatch.Stop();
        LoggerAI.Info($"Process for adding tower finished in {_stopwatch.ElapsedMilliseconds} ms");

        var cmd = new StateChangeCommand
        {
          BattleId = battle.Id,
          TowerCreationOptions = towerToAdd.HasValue
            ? new[] {new TowerCreationOption {Position = towerToAdd.Value.position, Type = towerToAdd.Value.type}}
            : null,
          UnitCreationOptions = unitToAdd.HasValue
            ? new[] {new UnitCreationOption {PathId = unitToAdd.Value.pathId, Type = unitToAdd.Value.type}}
            : null,
        };
        await Task.WhenAll(_analyticsService.LogAsync(cmd), _liveBattleService.RecalculateAsync(cmd));
      }).ToArray());
    }
  }
}