using System;
using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.Effects;
using Towerland.GameServer.Common.Models.GameField;

namespace Towerland.GameServer.Common.Models.GameObjects
{
  public class GameObject: ICloneable, IEquatable<GameObject>
  {
    [JsonProperty("i")] public int GameId { set; get; }
    [JsonProperty("p")] public Point Position { get; set; }
    [JsonProperty("w")] public int WaitTicks { set; get; }
    [JsonProperty("e")] public SpecialEffect Effect { set; get; }
    [JsonProperty("t")] public GameObjectType Type { set; get; }

    protected GameObject()
    {
      Effect = SpecialEffect.Empty;
    }
    
    public GameObjectType ResolveType()
    {
      return ResolveType(Type);
    }

    public static GameObjectType ResolveType(GameObjectType type)
    {
      if(type >= GameObjectType.Reserved && type < GameObjectType.Castle)
        return GameObjectType.Reserved;

      if (type >= GameObjectType.Castle && type < GameObjectType.Whizzbang)
        return GameObjectType.Castle;

      if (type >= GameObjectType.Whizzbang && type < GameObjectType.Tower)
        return GameObjectType.Whizzbang;

      if (type >= GameObjectType.Tower && type < GameObjectType.Unit)
        return GameObjectType.Tower;

      if (type >= GameObjectType.Unit)
        return GameObjectType.Unit;

      return GameObjectType.Undefined;
    }

    public virtual object Clone()
    {
      return new GameObject
      {
        GameId = GameId,
        Position = Position,
        Type = Type,
        WaitTicks = WaitTicks,
        Effect = new SpecialEffect{Effect = Effect.Effect, Duration = Effect.Duration}
      };
    }

    #region Equals

    public bool Equals(GameObject other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return GameId == other.GameId;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != this.GetType()) return false;
      return Equals((GameObject) obj);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return base.GetHashCode();
      }
    }

    #endregion
  }
}
