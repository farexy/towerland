using System;
using System.Web.Http;
using Towerland.GameServer.Api.Controllers.Base;
using Towerland.GameServer.Api.Helpers;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Api.Controllers
{
  [RoutePrefix("user")]
  public class UserController : BaseAuthorizeController
  {
    private readonly IUserService _userService;

    public UserController(IUserService service)
    {
      _userService = service;
    }

    [HttpPost, Route("signin")]
    public string SignIn(SignInRequestModel requestModel)
    {
      var uid = _userService.CheckPassword(requestModel.Email, requestModel.Password);
      return uid == Guid.Empty
        ? string.Empty
        : UserSessionHelper.GetSessionHash(uid);
    }

    [HttpPost, Route("signup")]
    public string SignUp(SignUpRequestModel requestModel)
    {
      return UserSessionHelper.GetSessionHash(
        _userService.SignUp(requestModel.Email, requestModel.FullName, requestModel.Password, requestModel.Nickname));
    }

    [HttpGet, Route("exp")]
    public UserExperience GetExp()
    {
      return _userService.GetUserExpirience(UserSessionId);
    }

    [HttpGet, Route("rating")]
    public UserRating[] GetRating()
    {
      return _userService.GetUserRating();
    }
  }
}