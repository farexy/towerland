using System;
using System.Linq;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.DataAccess
{
  public class UserRepository : ICrudRepository<User>
  {
    private readonly IDbConnectionFactory _db;

    public UserRepository(IDbConnectionFactory db)
    {
      _db = db;
    }
    
    public User Get(Guid id)
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<User>(u => u.Id == id).SingleOrDefault();
      }
    }

    public Guid Add(User obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Insert(obj);
        return obj.Id;
      }
    }

    public void Update(Guid id, User obj)
    {
      throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
      throw new NotImplementedException();
    }

    public void Clear()
    {
      throw new NotImplementedException();
    }

    public User Get(int id)
    {
      throw new NotImplementedException();
    }

    public User[] Get(object[] ids)
    {
      throw new NotImplementedException();
    }

    public User[] Get()
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<User>().ToArray();
      }    
    }

    public int Create(IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Create(IGuidEntity entity)
    {
      throw new NotImplementedException();
    }

    public int Update(IIdentityEntity entity)
    {
      throw new NotImplementedException();
    }

    public Guid Update(IGuidEntity entity)
    {
      throw new NotImplementedException();
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