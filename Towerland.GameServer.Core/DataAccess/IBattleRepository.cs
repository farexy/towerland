using System;
using System.Threading.Tasks;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Core.DataAccess
{
  public interface IBattleRepository
  {
    Battle Find(Guid id);
    Task<Battle[]> GetByUserAsync(Guid userId);
    Guid Create(Battle obj);
    void Update(Battle obj);
  }
}