﻿using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Services;
using EtuStackOverflow.Controllers.Api.CustomControllerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers.Api
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController : CustomController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService=userService;
        }

        [HttpGet("profile-detail"), Authorize]
        public async Task<IActionResult> UserProfileDetailAsync()
        {
            var userId = GetUserId();

            var result = await _userService.UserProfileDetailAsync(int.Parse(userId));

            return CreateActionResultInstance(result);
        }

        [HttpPost("update-profile-detail"), Authorize]
        public async Task<IActionResult> UpdateProfileDetail([FromBody] UserProfileUpdateDto profileUpdateDto)
        {
            var userId = GetUserId();

            var result = await _userService.UpdateUserProfileDetailAsync(int.Parse(userId), profileUpdateDto);

            return CreateActionResultInstance(result);
        }
    }
}
