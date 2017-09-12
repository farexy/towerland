using System;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IBattleProvider
  {
    Guid InitNewBattle();
  }
}
