using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.Models;

namespace Towerland.GameServer.Controllers
{
  [Route("battlesearch")]
  public class BattleSearchController : BaseAuthorizeController
  {
    private readonly IBattleSearchService _battleSearchService;

    public BattleSearchController(IBattleSearchService battleSearchService)
    {
      _battleSearchService = battleSearchService;
    }

    [HttpGet("search")]
    public async Task Search()
    {
      await _battleSearchService.AddToQueueAsync(await UserSessionIdAsync);
    }

    [HttpGet("check")]
    public async Task<BattleSearchCheckResponseModel> Check()
    {
      return new BattleSearchCheckResponseModel
      {
        Found = _battleSearchService.TryGetBattle(await UserSessionIdAsync, out var battleId, out var side),
        BattleId = battleId,
        Side = side
      };
    }
  }
}