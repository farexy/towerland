using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.Models;
using Towerland.GameServer.Models.State;

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

    [HttpPost("")]
    public async Task Search()
    {
      await _battleSearchService.AddToQueueAsync(await UserSessionIdAsync);
    }

    [HttpPost("multibattle")]
    public async Task SearchMultiBattle()
    {
      await _battleSearchService.AddToMultiBattleQueueAsync(await UserSessionIdAsync);
    }

    [HttpGet("check")]
    public async Task<BattleSearchCheckResponseModel> Check()
    {
      return new BattleSearchCheckResponseModel
      {
        Found = _battleSearchService.TryGetBattle(await UserSessionIdAsync, out (Guid battleId, PlayerSide side) playerSettings),
        BattleId = playerSettings.battleId,
        Side = playerSettings.side
      };
    }
  }
}