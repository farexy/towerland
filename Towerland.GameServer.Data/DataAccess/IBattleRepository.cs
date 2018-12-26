using System;
using System.Threading.Tasks;
using Towerland.GameServer.Data.Entities;

namespace Towerland.GameServer.Data.DataAccess
{
  public interface IBattleRepository
  {
    Task<Battle> FindAsync(Guid id);
    Task<Battle[]> GetByUserAsync(Guid userId);
    Task<Guid> CreateAsync(Battle obj);
    Task UpdateAsync(Battle obj);
    Task UpdateMultiBattle(Guid battleId, MultiBattleInfo multiBattleInfo);
  }

  public class FakeBattleRepository : IBattleRepository
  {
    public Task<Battle> FindAsync(Guid id)
    {
      return Task.FromResult(new Battle());
    }

    public Task<Battle[]> GetByUserAsync(Guid userId)
    {
      return Task.FromResult(new Battle[0]);
    }

    public Task<Guid> CreateAsync(Battle obj)
    {
      return Task.FromResult(Guid.Empty);
    }

    public Task UpdateAsync(Battle obj)
    {
      return Task.Run(() => { });
    }

    public Task UpdateMultiBattle(Guid battleId, MultiBattleInfo multiBattleInfo)
    {
      return Task.CompletedTask;
    }
  }
}