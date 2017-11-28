using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public class Castle : GameObject
  {
    public Castle() : base()
    {
      Type = GameObjectType.Castle;
    }

    [JsonProperty("h")] public int Health { set; get; }
  }
}
