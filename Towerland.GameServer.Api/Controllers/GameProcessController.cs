using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Towerland.GameServer.Api.Controllers.Base;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.State;
using Towerland.GameServer.Domain.Helpers;
using Towerland.GameServer.Domain.Interfaces;

namespace Towerland.GameServer.Api.Controllers
{
  [RoutePrefix("game")]
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

    [HttpGet]
    [Route("{battle:guid}/init")]
    public Field GetField(Guid battle)
    {
      return _liveBattleService.GetField(battle);
    }
    
    [HttpGet]
    [Route("{battle:guid}/checkstate/{v:int}")]
    public HttpResponseMessage CheckBattleStateChanged(Guid battle, int v)
    {
      return new HttpResponseMessage
      {
        Content = new StringContent(_liveBattleService.CheckChanged(battle, v)
          ? new ActionsResponseModel
            {
              ActionsByTicks = _liveBattleService.GetCalculatedActionsByTicks(battle),
              State = _liveBattleService.GetFieldState(battle),
              Revision = _liveBattleService.GetRevision(battle)
            }.ToJsonString()
          : string.Empty)
      };
    }

    [HttpGet]
    [Route("{battle:guid}/tryend/")]
    public async Task TryEndBattle(Guid battle)
    {
      await _liveBattleService.TryEndBattleAsync(battle, UserSessionId);
    }
    
    [HttpPost]
    [Route("command")]
    public async Task PostCommand(StateChangeCommandRequestModel requestModel)
    {
      await _liveBattleService.RecalculateAsync(_mapper.Map<StateChangeCommand>(requestModel), requestModel.CurrentTick);
    }
  }
}