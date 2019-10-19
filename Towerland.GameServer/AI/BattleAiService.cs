using System;
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
  public class BattleAiService : BackgroundService
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(3);
    private DateTime _lastRun;

    private readonly IProvider<LiveBattleModel> _liveBattleProvider;
    private readonly ILiveBattleService _liveBattleService;
    private readonly IUnitSelector _unitSelector;
    private readonly ITowerSelector _towerSelector;

    public BattleAiService(IProvider<LiveBattleModel> liveBattleProvider, ILiveBattleService liveBattleService, 
      IUnitSelector unitSelector, ITowerSelector towerSelector)
    {
      _liveBattleProvider = liveBattleProvider;
      _liveBattleService = liveBattleService;
      _unitSelector = unitSelector;
      _towerSelector = towerSelector;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while (!stoppingToken.IsCancellationRequested)
      {
        try
        {
          if (DateTime.Now - Interval > _lastRun)
          {
            await Task.WhenAll(_liveBattleProvider.GetAll().Select(async battle =>
            {
              var battleCopy = battle.CreateCopy();
              _liveBattleService.ResolveActions(battleCopy);
              var towerToAdd = await Task.Run(() => _towerSelector.GetNewTower(battleCopy.State), stoppingToken);
              var unitToAdd = await Task.Run(() => _unitSelector.GetNewUnit(battleCopy.State), stoppingToken);
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
              await _liveBattleService.RecalculateAsync(cmd);
            }).ToArray());
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
  }
}