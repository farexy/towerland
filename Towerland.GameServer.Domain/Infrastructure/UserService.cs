using System;
using System.Linq;
using System.Text.RegularExpressions;
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

    public async Task<Guid> CheckPasswordAsync(string emailOrLogin, string password)
     {
      var entity = await _userRepository.FindEmailOrLoginAsync(emailOrLogin);
      if (entity == null)
      {
        throw new LogicException("Email or login isn't exists");
      }

      //return entity.Id;
      return entity.Password.SequenceEqual(password.ToPwdHash()) ? entity.Id : Guid.Empty;
    }

    public async Task<Guid> SignUpAsync(string email, string name, string pwd, string nickname)
    {
      var id = Guid.NewGuid();
      var pwdHash = pwd.ToPwdHash();
      var newUser = new User
      {
        Id = id,
        Email = email,
        FullName = name,
        Nickname = nickname,
        Password = pwdHash
      };
      await CheckUser(newUser);

      return await _userRepository.CreateAsync(newUser);
    }

    public Task<User> GetUserAsync(Guid id)
    {
      return _userRepository.FindAsync(id);
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

    #region Validations

    private const string EmailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

    private async Task CheckUser(User user)
    {
      var emailRegex = new Regex(EmailPattern);
      if (!emailRegex.IsMatch(user.Email))
      {
        throw new BusinessLogicException("Wrong email format");
      }

      if (emailRegex.IsMatch(user.Nickname))
      {
        throw new BusinessLogicException("Wrong nickname format");
      }

      var existingUser = await _userRepository.FindEmailOrLoginAsync(user.Email);
      if (existingUser != null)
      {
        throw new BusinessLogicException("User with same email already exists");
      }

      existingUser = await _userRepository.FindEmailOrLoginAsync(user.Nickname);
      if (existingUser != null)
      {
        throw new BusinessLogicException("User with same login already exists");
      }
    }

    #endregion
  }
}