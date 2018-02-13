using System;
using System.Linq;
using System.Threading.Tasks;
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
    
    public async Task<User> FindAsync(Guid id)
    {
      using (var cx = _db.OpenDbConnection())
      {
        return (await cx.SelectAsync<User>(u => u.Id == id)).SingleOrDefault();
      }
    }
    
    public async Task<User[]> GetAsync()
    {
      using (var cx = _db.OpenDbConnection())
      {
        return (await cx.SelectAsync<User>()).ToArray();
      }
    }

    public async Task<Guid> CreateAsync(User obj)
    {
      using (var cx = _db.OpenDbConnection())
      {
        await cx.InsertAsync(obj);
        return obj.Id;
      }
    }

    public async Task IncrementExperienceAsync(Guid id, int exp)
    {
      using (var cx = _db.OpenDbConnection())
      {
        await cx.UpdateAddAsync(() => new User {Experience = exp}, where: u => u.Id == id);
      }
    }
  }
}