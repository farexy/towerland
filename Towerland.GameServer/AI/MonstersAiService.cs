﻿using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.AI
{
    public class MonstersAiService : BattleAiService
    {
        protected static readonly ILog LoggerAI = LogManager.GetLogger(Assembly.GetEntryAssembly(), "Monsters.AI");

        private readonly IUnitSelector _unitSelector;

        public MonstersAiService(
            IProvider<LiveBattleModel> liveBattleProvider,
            ILiveBattleService liveBattleService,
            IUnitSelector unitSelector,
            IAnalyticsService analyticsService)
            : base(liveBattleProvider, liveBattleService, analyticsService, PlayerSide.Monsters)
        {
            _unitSelector = unitSelector;
        }

        protected override async Task<StateChangeCommand> GetCmdAsync(LiveBattleModel battle, LiveBattleModel battleCopy, CancellationToken stoppingToken)
        {
            LoggerAI.Info("Process for adding unit started");
            Stopwatch.Restart();
            var unitToAdd = await Task.Run(() => _unitSelector.GetNewUnit(battleCopy.State), stoppingToken);
            Stopwatch.Stop();
            LoggerAI.Info($"Process for adding unit finished in {Stopwatch.ElapsedMilliseconds} ms. Memory usage: {CurrentMemoryUsageKb} Kb");

            var cmd = new StateChangeCommand
            {
                BattleId = battle.Id,
                UnitCreationOptions = unitToAdd.HasValue
                    ? new[] {new UnitCreationOption {PathId = unitToAdd.Value.pathId, Type = unitToAdd.Value.type}}
                    : null,
            };
            return cmd;
        }
    }
}