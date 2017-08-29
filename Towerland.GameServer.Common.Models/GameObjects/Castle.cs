using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public class Castle : GameObject
  {
    public Castle()
    {
      Type = GameObjectType.Castle;
    }

    [JsonProperty("h")] public int Health { set; get; }
  }
}
