using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.AI
{
    public class TowersAiService : BattleAiService
    {
        protected static readonly ILog LoggerAI = LogManager.GetLogger(Assembly.GetEntryAssembly(), "AI.Towers");

        private readonly ITowerSelector _towerSelector;

        public TowersAiService(
            IProvider<LiveBattleModel> liveBattleProvider,
            ILiveBattleService liveBattleService,
            ITowerSelector towerSelector,
            IAnalyticsService analyticsService)
            : base(liveBattleProvider, liveBattleService, analyticsService, PlayerSide.Towers)
        {
            _towerSelector = towerSelector;
        }

        protected override async Task<StateChangeCommand> GetCmdAsync(LiveBattleModel battle, LiveBattleModel battleCopy, CancellationToken stoppingToken)
        {
            LoggerAI.Info("Process for adding tower started");
            Stopwatch.Restart();
            var towerToAdd = await Task.Run(() => _towerSelector.GetNewTower(battleCopy.State), stoppingToken);
            Stopwatch.Stop();
            LoggerAI.Info($"Process for adding tower finished in {Stopwatch.ElapsedMilliseconds} ms");

            var cmd = new StateChangeCommand
            {
                BattleId = battle.Id,
                TowerCreationOptions = towerToAdd.HasValue
                    ? new[] {new TowerCreationOption {Position = towerToAdd.Value.position, Type = towerToAdd.Value.type}}
                    : null,
            };

            return cmd;
        }
    }
}