using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.Effects;

namespace Towerland.GameServer.Common.Models.GameObjects
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
        Effect = new SpecialEffect{Id = Effect.Id, Duration = Effect.Duration},
        Health = Health,
        PathId = PathId
      };
    }
  }
}
