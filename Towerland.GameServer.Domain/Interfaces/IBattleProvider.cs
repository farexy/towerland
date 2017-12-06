using System;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IBattleService
  {
    Guid InitNewBattle(Guid monstersPlayer, Guid towersPlayer);
  }
}
