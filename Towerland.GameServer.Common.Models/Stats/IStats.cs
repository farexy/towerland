using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Models.Stats
{
  public interface IStats
  {
    GameObjectType Type { get; set; }
  }
}