using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using GameServer.Api.Helpers;
using GameServer.Api.Models;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.State;
using Towerland.GameServer.Domain.Interfaces;

namespace GameServer.Api.Controllers
{
  [RoutePrefix("game")]
  public class GameProcessController : ApiController
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

    [HttpGet]
    [Route("{battle:guid}/check/{v:int}")]
    public bool CheckBattleStateChanged(Guid battle, int v)
    {
      return _liveBattleService.CheckChanged(battle, v);
    }

    [HttpGet]
    [Route("{battle:guid}/ticks")]
    public ActionsResponseModel GetActionsByTicks(Guid battle)
    {
      return new ActionsResponseModel
      {
        ActionsByTicks = _liveBattleService.GetCalculatedActionsByTicks(battle),
        State = _liveBattleService.GetFieldState(battle),
        Revision = _liveBattleService.GetRevision(battle)
      };
    }

    [HttpGet]
    [Route("{battle:guid}/tryend/{user:guid}")]
    public async Task TryEndBattle(Guid battle, Guid user)
    {
      await _liveBattleService.TryEndBattleAsync(battle, user);
    }
    
    [HttpPost]
    [Route("command")]
    public async Task PostCommand(StateChangeCommandRequestModel requestModel)
    {
      await _liveBattleService.RecalculateAsync(_mapper.Map<StateChangeCommand>(requestModel), requestModel.CurrentTick);
    }
  }
}