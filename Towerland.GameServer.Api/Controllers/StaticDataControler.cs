using System.Web.Http;
using AutoMapper;
using Towerland.GameServer.Api.Controllers.Base;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Common.Logic.Factories;

namespace Towerland.GameServer.Api.Controllers
{
    [RoutePrefix("data")]
    public class StaticDataController : BaseAuthorizeController
    {
        private readonly IMapper _mapper;
        
        private readonly StatsFactory _fatory = new StatsFactory();

        public StaticDataController(IMapper mapper)
        {
            _mapper = mapper;
        }
        
        [HttpGet, Route("stats")]
        public StatsResponseModel GetStats()
        {
            return _mapper.Map<StatsResponseModel>(_fatory);
        }
    }
}