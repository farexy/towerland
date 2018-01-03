using System;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IUserService
  {
    Guid CheckPassword(string email, string password);
    Guid SignUp(string email, string name, string pwd, string nickname);
    
    UserRating[] GetUserRating();
    UserExperience GetUserExpirience(Guid id);
  }
}