using System;
using System.Threading.Tasks;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Core.DataAccess
{
  public interface IUserRepository
  {
    Task<User> FindAsync(Guid id);
    Task<User[]> GetAsync();
    
    Task<Guid> CreateAsync(User obj);
    Task IncrementExperienceAsync(Guid id, int exp);
  }
}