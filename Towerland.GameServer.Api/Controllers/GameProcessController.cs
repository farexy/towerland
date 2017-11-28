using System;
using System.Web.Http;
using Towerland.GameServer.Domain.Interfaces;

namespace GameServer.Api.Controllers
{
  [RoutePrefix("game")]
  public class GameProcessController : ApiController
  {
    private IBattleProvider _battleProvider;
    private IBattleSearchService _battleSearchService;
    private ILiveBattleService _liveBattleService;

    public GameProcessController(IBattleProvider battleProvider, IBattleSearchService battleSearchService,
      ILiveBattleService liveBattleService)
    {
      _battleProvider = battleProvider;
      _battleSearchService = battleSearchService;
      _liveBattleService = liveBattleService;
    }

    [HttpGet]
    [Route("check")]
    public bool CheckbattleStateChanged(Guid battle, int v)
    {
      return _liveBattleService.CheckChanged(battle, v);
    }
    
    //[HttpGet]
    //[Route("state")]
    //public Fie
  }
}