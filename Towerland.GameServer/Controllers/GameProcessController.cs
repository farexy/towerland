using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Towerland.GameServer.BusinessLogic.Helpers;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.Models;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Controllers
{
  [Route("game")]
  public class GameProcessController : BaseAuthorizeController
  {
    private readonly ILiveBattleService _liveBattleService;
    private readonly IMapper _mapper;

    public GameProcessController(
      ILiveBattleService liveBattleService,
      IMapper mapper)
    {
      _liveBattleService = liveBattleService;
      _mapper = mapper;
    }

    [HttpGet("{battle:guid}/init")]
    public Field GetField(Guid battle)
    {
      return _liveBattleService.GetField(battle);
    }

    [HttpGet("{battle:guid}/checkstate/{v:int}")]
    public string CheckBattleStateChanged(Guid battle, int v)
    {
      var content = string.Empty;
      if (_liveBattleService.CheckChanged(battle, v))
      {
        var actions = _mapper.Map<ActionsResponseModel>(_liveBattleService.GetActualBattleState(battle, out var revision));
        actions.Revision = revision;
        content = actions.ToJsonString();
      }

      return content;
    }

    [HttpGet("{battle:guid}/tryend/")]
    public async Task TryEndBattle(Guid battle)
    {
      await _liveBattleService.TryEndBattleAsync(battle, await UserSessionIdAsync);
    }

    [HttpPost("command")]
    public Task PostCommand([FromBody] StateChangeCommandRequestModel requestModel)
    {
      return _liveBattleService.RecalculateAsync(_mapper.Map<StateChangeCommand>(requestModel), requestModel.CurrentTick);
    }
  }
}