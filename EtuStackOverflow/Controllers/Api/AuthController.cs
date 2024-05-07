using AskForEtu.Core.Dto.Request;
using AskForEtu.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EtuStackOverflow.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService=authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                await _authService.Register(registerDto);
            }
            catch (Exception)
            {
                return BadRequest("Serverda hata olustu");
            }

            return Ok("Basarili");
        }
    }
}
