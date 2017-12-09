using GameServer.Common.Models.Effects;
using GameServer.Common.Models.GameField;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public abstract class GameObject
  {
    [JsonProperty("GameId")] public int GameId { set; get; }
    [JsonProperty("Position")] public Point Position { get; set; }
    [JsonProperty("WaitTicks")] public int WaitTicks { set; get; }
    [JsonProperty("Effect")] public SpecialEffect Effect { set; get; }
    [JsonProperty("Type")] public GameObjectType Type { set; get; }

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
