using System;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IProvider<T>
  {
    Guid Create(LiveBattleModel obj);
    LiveBattleModel Find(Guid id);
    void Update(LiveBattleModel obj);
    void Delete(Guid id);
    void Clear();
  }
}