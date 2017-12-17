using System;
using GameServer.Common.Models.Effects;
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
    
    public override object Clone()
    {
      return new Castle
      {
        GameId = GameId,
        Position = Position,
        Type = Type,
        WaitTicks = WaitTicks,
        Effect = new SpecialEffect{Effect = Effect.Effect, Duration = Effect.Duration},
        Health = Health
      };
    }
  }
}
