using Newtonsoft.Json;
using Towerland.GameServer.Models.Effects;

namespace Towerland.GameServer.Models.GameObjects
{
  public class Castle : GameObject
  {    
    public Castle() : base()
    {
      Type = GameObjectType.Castle;
    }

    [JsonProperty("h")] public int Health { set; get; }
    
    public override object Clone()
    {
      return new Castle
      {
        GameId = GameId,
        Position = Position,
        Type = Type,
        WaitTicks = WaitTicks,
        Effect = new SpecialEffect{Id = Effect.Id, Duration = Effect.Duration},
        Health = Health
      };
    }
  }
}
