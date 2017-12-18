using System;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IUserService
  {
    bool CheckPassword(string email, string password);

    UserRating[] GetUserRating();
    UserExperience GetUserExpirience(Guid id);
  }
}