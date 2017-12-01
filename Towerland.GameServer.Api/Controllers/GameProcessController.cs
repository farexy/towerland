using System;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using GameServer.Api.Models;
using GameServer.Common.Models.GameActions;
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
    public bool CheckbattleStateChanged(Guid battle, int v)
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
          .Select(items => items.Select(a => _mapper.Map<GameAction>(a)))
      };
    }
    //[HttpGet]
    //[Route("state")]
    //public Fie
  }
}