using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public class Unit : GameObject
  {
    public Unit() : base()
    {
      Type = GameObjectType.Unit;
    }

    [JsonProperty("Health")] public int Health { set; get; }
    [JsonProperty("PathId")] public int? PathId { set; get; }
  }
}
