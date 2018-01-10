using System;
using System.Linq;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Core.DataAccess
{
  public class UserRepository : IUserRepository
  {
    private readonly IDbConnectionFactory _db;

    public UserRepository(IDbConnectionFactory db)
    {
      _db = db;
    }
    
    public User Find(Guid id)
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<User>(u => u.Id == id).SingleOrDefault();
      }
    }
    
    public User[] Get()
    {
      using (var cx = _db.OpenDbConnection())
      {
        return cx.Select<User>().ToArray();
      }    
    }

    public Guid Create(User obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.Insert(obj);
        return obj.Id;
      }
    }

    public void IncrementExperience(Guid id, int exp)
    {
      using (var cx = _db.OpenDbConnection())
      {
        cx.UpdateAdd(() => new User {Experience = exp}, where: u => u.Id == id);
      }
    }
  }
}