using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Services;
using EtuStackOverflow.Controllers.Api.CustomControllerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : CustomController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService=authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            return CreateActionResultInstance(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            return CreateActionResultInstance(result);
        }

        [HttpPost("logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = GetUserId();

            var response = await _authService.LogoutUserAsync(int.Parse(userId));

            return CreateActionResultInstance(response);
        }

        [Route("/api/verify-email/{t}")]
        [HttpGet]
        public async Task<IActionResult> VerifyEmail([FromRoute(Name = "t")]string t)
        {
            var result = await _authService.VerifyEmailRequestAsync(t);

            if (result.IsSuccessful)
            {
                return Redirect("/email/islem_basarili");
            }

            return Redirect("/email/islem_basarisiz");
        }
    }
}
