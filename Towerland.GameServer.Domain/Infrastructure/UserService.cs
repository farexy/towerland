using System.Linq;
using GameServer.Common.Models.Exceptions;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class UserService : IUserService
  {
    private readonly ICrudRepository<User> _userRepository;
    private readonly ICrudRepository<Battle> _battleRepository;

    public UserService(ICrudRepository<User> userRepository, ICrudRepository<Battle> battleRepository)
    {
      _userRepository = userRepository;
      _battleRepository = battleRepository;
    }
    
    public bool CheckPassword(string email, string password)
    {
      var entity = _userRepository.Get().FirstOrDefault(u => u.Email == email);
      if (entity == null)
      {
        throw new LogicException("Email isn't exists");
      }
      return entity.Password == password;
    }

    public UserRating[] GetUserRating()
    {
      var users = _userRepository.Get();
      var rating = users.Select(CalcRating).OrderByDescending(u => u.RatingPoints).ToArray();
      for (int i = 0; i < rating.Length; i++)
      {
        rating[i].Position = i + 1;
      }
      return rating;
    }

    public UserExperience GetUserExpirience()
    {
      throw new System.NotImplementedException();
    }

    private static UserRating CalcRating(User user)
    {
      return new UserRating
      {

      };
    }
  }
}