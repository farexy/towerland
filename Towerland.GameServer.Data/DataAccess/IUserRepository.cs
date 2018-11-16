using System;
using System.Threading.Tasks;
using Towerland.GameServer.Data.Entities;

namespace Towerland.GameServer.Data.DataAccess
{
  public interface IUserRepository
  {
    Task<User> FindAsync(Guid id);
    Task<User> FindEmailOrLoginAsync(string emailOrLogin);
    Task<User[]> GetAsync();

    Task<Guid> CreateAsync(User obj);
    Task IncrementExperienceAsync(Guid id, int exp);
  }

  public class FakeUserRepository : IUserRepository
  {
    public Task<User> FindAsync(Guid id)
    {
      return Task.FromResult(new User());
    }

    public Task<User> FindEmailOrLoginAsync(string emailOrLogin)
    {
      return Task.FromResult(new User());
    }

    public Task<User[]> GetAsync()
    {
      return Task.FromResult(new User[0]);
    }

    public Task<Guid> CreateAsync(User obj)
    {
      return Task.FromResult(Guid.Empty);
    }

    public Task IncrementExperienceAsync(Guid id, int exp)
    {
      return Task.Run(() => { });
    }
  }
}