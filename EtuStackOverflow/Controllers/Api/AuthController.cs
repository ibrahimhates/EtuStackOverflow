using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Services;
using EtuStackOverflow.Controllers.Api.CustomControllerBase;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);

            return CreateActionResultInstance(result);
        }

        [Route("/api/verify-email")]
        [HttpGet]
        public async Task<IActionResult> VerifyEmail([FromQuery]string t)
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
