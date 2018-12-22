using System;
using Newtonsoft.Json;
using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Models.GameObjects
{
  public class GameObject: ICloneable, IEquatable<GameObject>
  {
    [JsonProperty("i")] public int GameId { set; get; }
    [JsonProperty("p")] public Point Position { get; set; }
    [JsonProperty("w")] public int WaitTicks { set; get; }
    [JsonProperty("e")] public SpecialEffect Effect { set; get; }
    [JsonProperty("t")] public GameObjectType Type { set; get; }

    public GameObject()
    {
      Effect = SpecialEffect.Empty;
    }

    [JsonIgnore] public bool IsTower => ResolveType() == GameObjectType.Tower;

    [JsonIgnore] public bool IsUnit => ResolveType() == GameObjectType.Unit;

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

      if (type >= GameObjectType.Whizzbang && type < GameObjectType.Explosion)
        return GameObjectType.Whizzbang;
      
      if (type >= GameObjectType.Explosion && type < GameObjectType.Tower)
        return GameObjectType.Explosion;

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
        Effect = new SpecialEffect{Id = Effect.Id, Duration = Effect.Duration}
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
      return GameId;
    }

    #endregion
  }
}
