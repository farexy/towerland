﻿using System;
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
  
  public class FakeUserRepository : IUserRepository
  {
    public Task<User> FindAsync(Guid id)
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