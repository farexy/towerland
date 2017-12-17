using GameServer.Common.Models.Effects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public class Unit : GameObject
  {
    public Unit() : base()
    {
      Type = GameObjectType.Unit;
    }

    [JsonProperty("h")] public int Health { set; get; }
    [JsonProperty("z")] public int? PathId { set; get; }
    
    public override object Clone()
    {
      return new Unit
      {
        GameId = GameId,
        Position = Position,
        Type = Type,
        WaitTicks = WaitTicks,
        Effect = new SpecialEffect{Effect = Effect.Effect, Duration = Effect.Duration},
        Health = Health,
        PathId = PathId
      };
    }
  }
}
