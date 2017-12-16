using System.Collections.Generic;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using Newtonsoft.Json;

namespace GameServer.Api.Models
{
  public class ActionsResponseModel
  {
    [JsonProperty("revision")]public int Revision { get; set; }
    [JsonProperty("state")]public FieldState State { get; set; }
    [JsonProperty("actionsByTick")]public IEnumerable<GameTick> ActionsByTicks { get; set; }
  }
}