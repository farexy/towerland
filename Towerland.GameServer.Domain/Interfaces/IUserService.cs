using System;
using System.Threading.Tasks;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Interfaces
{
  public interface IUserService
  {
    Task<Guid> CheckPasswordAsync(string email, string password);
    Task<Guid> SignUpAsync(string email, string name, string pwd, string nickname);
    
    Task<UserRating[]> GetUserRatingAsync();
    Task<UserExperience> GetUserExpirienceAsync(Guid id);
  }
}