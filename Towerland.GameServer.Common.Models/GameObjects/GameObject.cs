using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameField;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public abstract class GameObject
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

      if (type >= GameObjectType.Castle && type < GameObjectType.Tower)
        return GameObjectType.Castle;

      if (type >= GameObjectType.Tower && type < GameObjectType.Unit)
        return GameObjectType.Tower;

      if (type >= GameObjectType.Unit)
        return GameObjectType.Unit;

      return GameObjectType.Undefined;
    }
  }
}
