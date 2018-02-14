using System;
using System.Threading.Tasks;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Core.DataAccess
{
  public interface IBattleRepository
  {
    Task<Battle> FindAsync(Guid id);
    Task<Battle[]> GetByUserAsync(Guid userId);
    Task<Guid> CreateAsync(Battle obj);
    Task UpdateAsync(Battle obj);
  }
}