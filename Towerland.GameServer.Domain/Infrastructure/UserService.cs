using System;
using System.Linq;
using System.Threading.Tasks;
using Towerland.GameServer.Common.Models.Exceptions;
using Towerland.GameServer.Core.DataAccess;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Helpers;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    private readonly IBattleRepository _battleRepository;

    public UserService(IUserRepository userRepository, IBattleRepository battleRepository)
    {
      _userRepository = userRepository;
      _battleRepository = battleRepository;
    }
    
    public async Task<Guid> CheckPasswordAsync(string email, string password)
     {
      var entity = (await _userRepository.GetAsync()).FirstOrDefault(u => u.Email == email);
      if (entity == null)
      {
        throw new LogicException("Email isn't exists");
      }
      return entity.Password.SequenceEqual(password.ToPwdHash()) ? entity.Id : Guid.Empty;
    }

    public async Task<Guid> SignUpAsync(string email, string name, string pwd, string nickname)
    {
      var id = Guid.NewGuid();
      var pwdHash = pwd.ToPwdHash();
      return await _userRepository.CreateAsync(new User
      {
        Id = id,
        Email = email,
        FullName = name,
        Nickname = nickname,
        Password = pwdHash
      });
    }

    public async Task<UserRating[]> GetUserRatingAsync()
    {
      var users = await _userRepository.GetAsync();
      var userRating = new UserRating[users.Length];
      for (int i = 0; i < userRating.Length; i++)
      {
        userRating[i] = await CalcRatingAsync(users[i]);
      }
      var rating = userRating.OrderByDescending(u => u.RatingPoints).ToArray();
      for (int i = 0; i < rating.Length; i++)
      {
        rating[i].Position = i + 1;
      }

      GC.Collect();
      return rating;
    }

    public async Task<UserExperience> GetUserExpirienceAsync(Guid id)
    {
      var exp = (await _userRepository.FindAsync(id)).Experience;
      return new UserExperience
      {
        Experience = exp,
        RelativeExperience = exp % 100,
        Level = 1 + exp / 100,
        TotalLevelExperience = 100
      };
    }

    private async Task<UserRating> CalcRatingAsync(User user)
    {
      var battles = await _battleRepository.GetByUserAsync(user.Id);
      var exp = 0;
      var wins = 0;
      foreach (var b in battles)
      {
        exp += user.Experience;
        wins += b.IsWinner(user.Id) ? 1 : 0;
      }
      return new UserRating
      {
        UserId = user.Id,
        Nickname = user.Nickname,
        Expirience = exp,
        VictoryPercent = (int)((double)wins / battles.Length * 100),
        RatingPoints = exp * wins / battles.Length
      };
    }

   
  }
}