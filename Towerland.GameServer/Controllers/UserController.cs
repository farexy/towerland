using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Towerland.GameServer.BusinessLogic.Interfaces;
using Towerland.GameServer.BusinessLogic.Models;
using Towerland.GameServer.Helpers;
using Towerland.GameServer.Models;

namespace Towerland.GameServer.Controllers
{
  [Route("user")]
  public class UserController : BaseAuthorizeController
  {
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public UserController(IUserService service, IMapper mapper)
    {
      _mapper = mapper;
      _userService = service;
    }

    [HttpPost("signin")]
    public async Task<string> SignInAsync([FromBody] SignInRequestModel requestModel)
    {
      var uid = await _userService.CheckPasswordAsync(requestModel.Email, requestModel.Password);
      return uid == Guid.Empty
        ? string.Empty
        : await UserSessionHelper.GetSessionHashAsync(uid);
    }

    [HttpPost("signup")]
    public async Task<string> SignUpAsync([FromBody] SignUpRequestModel requestModel)
    {
      return await UserSessionHelper.GetSessionHashAsync(
        await _userService.SignUpAsync(requestModel.Email, requestModel.FullName, requestModel.Password, requestModel.Nickname));
    }

    [HttpGet]
    public async Task<UserDetailsResponseModel> GetDetails()
    {
      return _mapper.Map<UserDetailsResponseModel>(await _userService.GetUserAsync(await UserSessionIdAsync));
    }

    [HttpGet("exp")]
    public async Task<UserExperience> GetExpAsync()
    {
      return await _userService.GetUserExperienceAsync(await UserSessionIdAsync);
    }

    [HttpGet("rating")]
    public async Task<UserRating[]> GetRatingAsync()
    {
      return await _userService.GetUserRatingAsync();
    }
  }
}