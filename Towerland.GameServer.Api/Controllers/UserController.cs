using System;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Towerland.GameServer.Api.Controllers.Base;
using Towerland.GameServer.Api.Helpers;
using Towerland.GameServer.Api.Models;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Interfaces;
using Towerland.GameServer.Domain.Models;

namespace Towerland.GameServer.Api.Controllers
{
  [RoutePrefix("user")]
  public class UserController : BaseAuthorizeController
  {
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(IUserService service, IMapper mapper)
    {
      _mapper = mapper;
      _userService = service;
    }

    [HttpPost, Route("signin")]
    public async Task<string> SignInAsync(SignInRequestModel requestModel)
    {
      var uid = await _userService.CheckPasswordAsync(requestModel.Email, requestModel.Password);
      return uid == Guid.Empty
        ? string.Empty
        : await UserSessionHelper.GetSessionHashAsync(uid);
    }

    [HttpPost, Route("signup")]
    public async Task<string> SignUpAsync(SignUpRequestModel requestModel)
    {
      return await UserSessionHelper.GetSessionHashAsync(
        await _userService.SignUpAsync(requestModel.Email, requestModel.FullName, requestModel.Password, requestModel.Nickname));
    }

    [HttpGet, Route]
    public async Task<UserDetailsResponseModel> GetDetails()
    {
      return _mapper.Map<UserDetailsResponseModel>(await _userService.GetUserAsync(await UserSessionIdAsync));
    }

    [HttpGet, Route("exp")]
    public async Task<UserExperience> GetExpAsync()
    {
      return await _userService.GetUserExperienceAsync(await UserSessionIdAsync);
    }

    [HttpGet, Route("rating")]
    public async Task<UserRating[]> GetRatingAsync()
    {
      return await _userService.GetUserRatingAsync();
    }
  }
}