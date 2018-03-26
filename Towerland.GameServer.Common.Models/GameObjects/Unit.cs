using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.Effects;

namespace Towerland.GameServer.Common.Models.GameObjects
{
  public class Unit : GameObject
  {
    public Unit()
    {
      Type = GameObjectType.Unit;
    }

    [JsonProperty("h")] public int Health { set; get; }
    [JsonProperty("z")] public int? PathId { set; get; }
    [JsonProperty("d")] public bool IsDead { set; get; }

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
        PathId = PathId,
        IsDead = IsDead
      };
    }
  }
}
