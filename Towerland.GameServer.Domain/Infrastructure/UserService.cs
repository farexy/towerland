using System;
using System.Linq;
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
    
    public Guid CheckPassword(string email, string password)
     {
      var entity = _userRepository.Get().FirstOrDefault(u => u.Email == email);
      if (entity == null)
      {
        throw new LogicException("Email isn't exists");
      }
      return entity.Password.SequenceEqual(password.ToPwdHash()) ? entity.Id : Guid.Empty;
    }

    public Guid SignUp(string email, string name, string pwd, string nickname)
    {
      var id = Guid.NewGuid();
      var pwdHash = pwd.ToPwdHash();
      return _userRepository.Create(new User
      {
        Id = id,
        Email = email,
        FullName = name,
        Nickname = nickname,
        Password = pwdHash
      });
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

    public UserExperience GetUserExpirience(Guid id)
    {
      var exp = _userRepository.Find(id).Experience;
      return new UserExperience
      {
        Experience = exp,
        RelativeExperience = exp % 100,
        Level = 1 + exp / 100,
        TotalLevelExperience = 100
      };
    }

    private UserRating CalcRating(User user)
    {
      var battles = _battleRepository.Get().Where(b => b.Monsters_UserId == user.Id || b.Towers_UserId == user.Id).ToArray();
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