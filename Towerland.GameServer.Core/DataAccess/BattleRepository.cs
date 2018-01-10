using System;
using System.Linq;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Core.DataAccess
{
  public class BattleRepository : IBattleRepository
  {
    private readonly IDbConnectionFactory _db;

    public BattleRepository(IDbConnectionFactory db)
    {
      _db = db;
    }
    
    public Battle Find(Guid id)
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<Battle>(u => u.Id == id).SingleOrDefault();
      }
    }
    
    public Battle[] Get()
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<Battle>().ToArray();
      }
    }

    public Guid Create(Battle obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Insert(obj);
        return obj.Id;
      }
    }

    public void Update(Battle obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Update(obj);
      }
    }
  }
}