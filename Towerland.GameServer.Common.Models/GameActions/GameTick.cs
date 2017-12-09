using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameActions
{
  public class GameTick
  {
    [JsonProperty("RelativeTime")] public int RelativeTime { get; set; }
    [JsonProperty("Actions")] public IEnumerable<GameAction> Actions { get; set; }

    [JsonIgnore]
    public bool HasNoActions
    {
      get { return Actions == null || !Actions.Any(); }
    }
  }
}
