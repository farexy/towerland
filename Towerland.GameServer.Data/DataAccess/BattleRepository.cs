using System;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Towerland.GameServer.Data.Entities;

namespace Towerland.GameServer.Data.DataAccess
{
  public class BattleRepository : IBattleRepository
  {
    private readonly IDbConnectionFactory _db;

    public BattleRepository(IDbConnectionFactory db)
    {
      _db = db;
    }

    public async Task<Battle> FindAsync(Guid id)
    {
      using (var cx = _db.OpenDbConnection())
      {
        return (await cx.SelectAsync<Battle>(u => u.Id == id)).SingleOrDefault();
      }
    }

    public async Task<Battle[]> GetByUserAsync(Guid userId)
    {
      using (var cx = _db.OpenDbConnection())
      {
        return (await cx.SelectAsync<Battle>(b => b.Monsters_UserId == userId || b.Towers_UserId == userId)).ToArray();
      }
    }

    public async Task<Guid> CreateAsync(Battle obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        await cx.InsertAsync(obj);
        return obj.Id;
      }
    }

    public async Task UpdateAsync(Battle obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        await cx.UpdateAsync(obj);
      }
    }
  }
}