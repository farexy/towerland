using System.Web.Http;
using AutoMapper;
using Towerland.GameServer.Api.Controllers.Base;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Common.Logic.Interfaces;

namespace Towerland.GameServer.Api.Controllers
{
    [RoutePrefix("data")]
    public class StaticDataController : BaseAuthorizeController
    {
        private readonly IMapper _mapper;
        private readonly IStatsProvider _statsProvider;

        public StaticDataController(IStatsProvider statsProvider, IMapper mapper)
        {
            _statsProvider = statsProvider;
            _mapper = mapper;
        }

        [HttpGet, Route("stats")]
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