using System;
using System.Web.Http;
using GameServer.Api.Controllers.Base;
using GameServer.Api.Models;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace GameServer.Api.Controllers
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
      return _userService.CheckPassword(requestModel.Email, requestModel.Password).ToString();
    }

    [HttpGet, Route("exp/{id:guid}")]
    public UserExperience GetExp(Guid id)
    {
      return _userService.GetUserExpirience(id);
    }

    [HttpGet, Route("rating")]
    public UserRating[] GetRating()
    {
      return _userService.GetUserRating();
    }
  }
}