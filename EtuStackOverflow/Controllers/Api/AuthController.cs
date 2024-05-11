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
        private readonly LinkGenerator _linkGenerator;
        public AuthController(IAuthService authService, LinkGenerator linkGenerator)
        {
            _authService=authService;
            _linkGenerator=linkGenerator;
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
            var result = await _authService.RegisterAsync(registerDto, HttpContext);

            return CreateActionResultInstance(result);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDto forgetPasswordDto)
        {
            var result = await _authService.ForgetPasswordAsync(forgetPasswordDto);

            return CreateActionResultInstance(result);
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyPasswordResetCode([FromBody] ForgetPasswordWithCodeDto forgetPasswordDto)
        {
            var result = await _authService.ForgetPasswordVerifyCodeAsync(forgetPasswordDto);

            return CreateActionResultInstance(result);
        }
        
        [HttpPost("change-password-reset")]
        public async Task<IActionResult> ChabgePasswordWithResetCode([FromBody]ChangePasswordWithResetDto changePasswordWith)
        {
            var result = await _authService.ChangePasswordWithResetMethodAsync(changePasswordWith);

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
        public async Task<IActionResult> VerifyEmail([FromRoute(Name = "t")] string t)
        {
            var result = await _authService.VerifyEmailRequestAsync(t);

            if (result.IsSuccessful)
            {
                return Redirect("/email/islem_basarili");
            }

            return Redirect("/email/islem_basarisiz");
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestMethod()
        {
            var baseUrl = _linkGenerator
                .GetUriByAction(HttpContext, action: "verify-email", controller: "api", values: null, scheme: Request.Scheme);

            return Ok(baseUrl);
        }
    }
}
