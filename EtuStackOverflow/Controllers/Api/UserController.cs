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

        [HttpGet("profile-detail"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UserProfileDetailAsync()
        {
            var userId = GetUserId();

            var result = await _userService.UserProfileDetailAsync(int.Parse(userId));

            return CreateActionResultInstance(result);
        }

        [HttpPost("update-profile-detail"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> UpdateProfileDetail([FromBody] UserProfileUpdateDto profileUpdateDto)
        {
            var userId = GetUserId();

            var result = await _userService.UpdateUserProfileDetailAsync(int.Parse(userId), profileUpdateDto);

            return CreateActionResultInstance(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser([FromQuery] int pageNumber, [FromQuery] string? searchTerm)
        {
            var result = await _userService.AllUserWithPaggingAsync(pageNumber,searchTerm);

            return CreateActionResultInstance(result);
        }

        [HttpGet("{userId:int}"), Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetOneUser([FromRoute] int userId)
        {
            var role = GetUserRole();

            var result = await _userService.GetOneUserDetailAsync(userId,role);

            return CreateActionResultInstance(result);
        }
        
        [HttpDelete("{userId:int}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] int userId)
        {
            var result = await _userService.DeleteOneUserAsync(userId);

            return CreateActionResultInstance(result);
        }
    }
}
