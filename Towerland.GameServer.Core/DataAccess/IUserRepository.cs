using System;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Core.DataAccess
{
  public interface IUserRepository
  {
    User Find(Guid id);
    User[] Get();
    
    Guid Create(User obj);
    void IncrementExperience(Guid id, int exp);
  }
}