using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.State
{
  public class TowerCreationOption
  {
    [JsonProperty("Type")] public GameObjectType Type { set; get; }
    [JsonProperty("Position")] public Point Position { set; get; }
  }
}
