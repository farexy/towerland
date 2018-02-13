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
      var content = string.Empty;
      if (_liveBattleService.CheckChanged(battle, v))
      {
        var actions = _mapper.Map<ActionsResponseModel>(_liveBattleService.GetActualBattleState(battle, out var revision));
        actions.Revision = revision;
        content = actions.ToJsonString();
      }
      
      return new HttpResponseMessage
      {
        Content = new StringContent(content)
      };
    }

    [HttpGet]
    [Route("{battle:guid}/tryend/")]
    public async Task TryEndBattle(Guid battle)
    {
      await _liveBattleService.TryEndBattleAsync(battle, await UserSessionIdAsync);
    }
    
    [HttpPost]
    [Route("command")]
    public async Task PostCommand(StateChangeCommandRequestModel requestModel)
    {
      await _liveBattleService.RecalculateAsync(_mapper.Map<StateChangeCommand>(requestModel), requestModel.CurrentTick);
    }
  }
}