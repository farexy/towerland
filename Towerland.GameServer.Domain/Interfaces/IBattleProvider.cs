using System;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IBattleInitializationService
  {
    Guid InitNewBattle(Guid monstersPlayer, Guid towersPlayer);
  }
}
