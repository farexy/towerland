using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Towerland.GameServer.BusinessLogic.Helpers;
using Towerland.GameServer.Helpers;
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

        [HttpGet("static")]
        public async Task<StaticDataResponseModel> GetStats()
        {
            return new StaticDataResponseModel
            {
                Stats = new StatsResponseModel
                {
                    UnitStats = _statsProvider.GetUnitStats(),
                    TowerStats = _statsProvider.GetTowerStats(),
                    DefenceCoeffs = _statsProvider.GetDefenceCoeffs(),
                    Skills = _statsProvider.GetSkills()
                },
                ServerTime = DateTime.UtcNow,
                ComputerPlayerSessionKey = await UserSessionHelper.GetSessionHashAsync(ComputerPlayer.Id)
            };
        }
    }
}