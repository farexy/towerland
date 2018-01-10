using System;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Core.DataAccess
{
  public interface IBattleRepository
  {
    Battle Find(Guid id);
    Battle[] Get();
    Guid Create(Battle obj);
    void Update(Battle obj);
  }
}