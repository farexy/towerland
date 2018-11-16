using System.Collections.Generic;
using Newtonsoft.Json;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Models
{
  public class ActionsResponseModel
  {
    [JsonProperty("revision")]public int Revision { get; set; }
    [JsonProperty("state")]public FieldState State { get; set; }
    [JsonProperty("actionsByTick")]public IEnumerable<GameTick> ActionsByTicks { get; set; }
  }
}