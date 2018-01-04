using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Models.State
{
  public class UnitCreationOption
  {
    [JsonProperty("type")] public GameObjectType Type { set; get; }
  }
}

