using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using GameServer.Api.Models;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.State;
using Towerland.GameServer.Domain.Interfaces;

namespace GameServer.Api.Controllers
{
  [RoutePrefix("game")]
  public class GameProcessController : ApiController
  {
    private IBattleService _battleProvider;
    private IBattleSearchService _battleSearchService;
    private readonly ILiveBattleService _liveBattleService;
    private readonly IMapper _mapper;

    public GameProcessController(IBattleService battleProvider, 
      IBattleSearchService battleSearchService,
      ILiveBattleService liveBattleService,
      IMapper mapper)
    {
      _battleProvider = battleProvider;
      _battleSearchService = battleSearchService;
      _liveBattleService = liveBattleService;
      _mapper = mapper;
    }

    [HttpGet]
    [Route("{battle:int}/check/{v:int}")]
    public bool CheckBattleStateChanged(Guid battle, int v)
    {
      return _liveBattleService.CheckChanged(battle, v);
    }

    [HttpGet]
    [Route("{battle:int}/actions")]
    public ActionsResponseModel GetActions(Guid battle)
    {
      return new ActionsResponseModel
      {
        ActionsByTicks = _liveBattleService.GetCalculatedActionsByTicks(battle)
      };
    }

    [HttpPost]
    [Route("command")]
    public async Task PostCommand([FromBody]StateChangeCommandRequestModel requestModel)
    {
      await _liveBattleService.RecalculateAsync(_mapper.Map<StateChangeCommand>(requestModel), requestModel.CurrentTick);
    }
    
    //[HttpGet]
    //[Route("state")]
    //public Fie
  }
}