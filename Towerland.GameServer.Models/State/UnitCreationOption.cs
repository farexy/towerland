using Newtonsoft.Json;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Models.State
{
  public class UnitCreationOption
  {
    [JsonProperty("type")] public GameObjectType Type { set; get; }
    [JsonProperty("pathId")] public int? PathId { set; get; }
  }
}

