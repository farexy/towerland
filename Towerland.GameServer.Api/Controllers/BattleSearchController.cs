using System;
using System.Threading.Tasks;
using System.Web.Http;
using GameServer.Api.Controllers.Base;
using GameServer.Api.Models;
using Towerland.GameServer.Domain.Interfaces;

namespace GameServer.Api.Controllers
{
  [RoutePrefix("battlesearch")]
  public class BattleSearchController : BaseAuthorizeController
  {
    private readonly IBattleSearchService _battleSearchService;

    public BattleSearchController(IBattleSearchService battleSearchService)
    {
      _battleSearchService = battleSearchService;
    }

    [HttpGet]
    [Route("search/{sessionId:guid}")]
    public async Task Search(Guid sessionId)
    {
      await _battleSearchService.AddToQueueAsync(sessionId);
    }

    [HttpGet]
    [Route("check/{sessionId:guid}")]
    public BattleSearchCheckResponseModel Check(Guid sessionId)
    {
      return new BattleSearchCheckResponseModel
      {
        Found = _battleSearchService.TryGetBattle(sessionId, out var battleId, out var side),
        BattleId = battleId,
        Side = side
      };
    }
  }
}