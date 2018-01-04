using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Models.State
{
  public class TowerCreationOption
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
    [JsonProperty("p")] public Point Position { set; get; }
  }
}
