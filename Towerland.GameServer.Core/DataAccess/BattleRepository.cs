using System;
using System.Linq;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.DataAccess
{
  public class BattleRepository : ICrudRepository<Battle>
  {
    private readonly IDbConnectionFactory _db;

    public BattleRepository(IDbConnectionFactory db)
    {
      _db = db;
    }
    
    public Battle Get(Guid id)
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<Battle>(u => u.Id == id).SingleOrDefault();
      }
    }

    public Guid Add(Battle obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Insert(obj);
        return obj.Id;
      }
    }

    public void Update(Guid id, Battle obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Update(obj);
      }
    }

    public void Delete(Guid id)
    {
      throw new NotImplementedException();
    }

    public Battle Get(int id)
    {
      throw new NotImplementedException();
    }

    public Battle[] Get(object[] ids)
    {
      throw new NotImplementedException();
    }

    public Battle[] Get()
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<Battle>().ToArray();
      }
    }

    public int Create(IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Create(IGuidEntity entity)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Insert((Battle)entity);
        return entity.Id;
      }    
    }

    public int Update(IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Update(IGuidEntity entity)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Update((Battle)entity);
        return entity.Id;
      }    
    }

    public int Update(int id, IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Update(Guid id, IGuidEntity entity)
    {
      throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
      throw new NotImplementedException();
    }

    public void SaveStateAsync()
    {
      throw new NotImplementedException();
    }
  }
}