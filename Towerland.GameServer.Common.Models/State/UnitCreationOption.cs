using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.State
{
  public class UnitCreationOption
  {
    [JsonProperty("t")] public GameObjectType Type { set; get; }
  }
}

