using System.Collections.Generic;
using System.Threading.Tasks;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.BusinessLogic.Infrastructure
{
    public class AnalyticsServiceStub : IAnalyticsService
    {
        private Dictionary<GameObjectType, int> _gameObjectStatistics = new Dictionary<GameObjectType, int>();
        private int _monsterWins;
        private int _towerWins;

        public Task LogAsync(StateChangeCommand cmd)
        {
            var tower = cmd.TowerCreationOptions;
            var unit = cmd.UnitCreationOptions;
            if (tower != null)
            {
                foreach (var opt in tower)
                {
                    if (_gameObjectStatistics.ContainsKey(opt.Type))
                    {
                        _gameObjectStatistics[opt.Type]++;
                    }
                }
            }

            if (unit != null)
            {
                foreach (var opt in unit)
                {
                    if (_gameObjectStatistics.ContainsKey(opt.Type))
                    {
                        _gameObjectStatistics[opt.Type]++;
                    }
                }
            }
            return Task.CompletedTask;
        }

        public Task LogEndAsync(PlayerSide winner, Field field)
        {
            switch (winner)
            {
                case PlayerSide.Monsters:
                    _monsterWins++;
                    break;
                case PlayerSide.Towers:
                    _towerWins++;
                    break;
                case PlayerSide.Both:
                    _monsterWins++;
                    _towerWins++;
                    break;
            }
            return Task.CompletedTask;
        }
    }
}