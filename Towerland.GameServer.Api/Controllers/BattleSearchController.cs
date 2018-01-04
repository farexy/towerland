using System.Threading.Tasks;
using System.Web.Http;
using Towerland.GameServer.Api.Controllers.Base;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Domain.Interfaces;

namespace Towerland.GameServer.Api.Controllers
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
    [Route("search")]
    public async Task Search()
    {
      await _battleSearchService.AddToQueueAsync(UserSessionId);
    }

    [HttpGet]
    [Route("check")]
    public BattleSearchCheckResponseModel Check()
    {
      return new BattleSearchCheckResponseModel
      {
        Found = _battleSearchService.TryGetBattle(UserSessionId, out var battleId, out var side),
        BattleId = battleId,
        Side = side
      };
    }
  }
}