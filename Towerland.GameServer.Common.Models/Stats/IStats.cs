using GameServer.Common.Models.GameObjects;

namespace GameServer.Common.Models.Stats
{
  public interface IStats
  {
    GameObjectType Type { get; set; }
  }
}