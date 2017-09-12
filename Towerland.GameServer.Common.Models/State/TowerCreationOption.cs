using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.State
{
  public class TowerCreationOption
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
    [JsonProperty("p")] public Point Position { set; get; }
  }
}
