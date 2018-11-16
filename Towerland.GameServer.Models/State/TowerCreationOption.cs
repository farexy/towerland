using Newtonsoft.Json;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Models.State
{
  public class TowerCreationOption
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
    [JsonProperty("p")] public Point Position { set; get; }
  }
}
