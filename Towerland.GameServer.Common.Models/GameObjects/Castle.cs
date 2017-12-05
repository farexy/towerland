using System;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public class Castle : GameObject, ICloneable
  {    
    public Castle() : base()
    {
      Type = GameObjectType.Castle;
    }

    [JsonProperty("h")] public int Health { set; get; }
    
    public object Clone()
    {
      return new Castle
      {
        GameId = GameId,
        Effect = Effect,
        Health = Health,
        Position = Position,
        Type = Type
      };
    }
  }
}
