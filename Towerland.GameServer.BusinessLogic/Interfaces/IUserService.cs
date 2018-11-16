using System;
using System.Threading.Tasks;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Data.Entities;

namespace Towerland.GameServer.BusinessLogic.Interfaces
{
  public interface IUserService
  {
    Task<Guid> CheckPasswordAsync(string emailOrLogin, string password);
    Task<Guid> SignUpAsync(string email, string name, string pwd, string nickname);
    
    Task<UserRating[]> GetUserRatingAsync();
    Task<UserExperience> GetUserExperienceAsync(Guid id);
    Task<User> GetUserAsync(Guid id);
  }
}