using Microsoft.AspNetCore.Mvc;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models;

namespace Towerland.GameServer.Controllers
{
    [Route("data")]
    public class StaticDataController : BaseAuthorizeController
    {
        private readonly IStatsProvider _statsProvider;

        public StaticDataController(IStatsProvider statsProvider)
        {
            _statsProvider = statsProvider;
        }

        [HttpGet("stats")]
        public StatsResponseModel GetStats()
        {
            return new StatsResponseModel
            {
                UnitStats = _statsProvider.GetUnitStats(),
                TowerStats = _statsProvider.GetTowerStats(),
                DefenceCoeffs = _statsProvider.GetDefenceCoeffs()
            };
        }
    }
}