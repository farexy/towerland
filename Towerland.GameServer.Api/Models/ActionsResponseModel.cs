using System.Collections.Generic;
using GameServer.Common.Models.GameActions;

namespace GameServer.Api.Models
{
  public class ActionsResponseModel
  {
    public IEnumerable<IEnumerable<GameAction>> ActionsByTicks { get; set; }
  }
}