using System;
using System.Collections.Generic;
using Towerland.GameServer.BusinessLogic.Models;

namespace Towerland.GameServer.BusinessLogic.Interfaces
{
  public interface IProvider<T>
  {
    IEnumerable<LiveBattleModel> GetAll();
    Guid Create(LiveBattleModel obj);
    LiveBattleModel Find(Guid id);
    void Update(LiveBattleModel obj);
    void Delete(Guid id);
    void Clear();
  }
}