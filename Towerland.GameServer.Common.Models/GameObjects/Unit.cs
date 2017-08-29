using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public class Unit : GameObject
  {
    public Unit()
    {
      Type = GameObjectType.Unit;
    }

    [JsonProperty("h")] public int Health { set; get; }
    [JsonProperty("z")] public int? PathId { set; get; }
  }
}
