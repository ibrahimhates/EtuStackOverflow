using AskForEtu.Core.ResultStructure;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace EtuStackOverflow.Controllers.Api.CustomControllerBase
{
    public class CustomController : ControllerBase
    {
        public IActionResult CreateActionResultInstance<T>(Response<T> response)
        {
            if (response.StatusCode == 204)
            {
                return NoContent();
            }

            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode
            };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUserId()
        {
            if (User.Claims.Any())
                return User.Claims.FirstOrDefault(x => x.Type.Equals("usrId")).Value;

            return null;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUserName()
        {
            if (HttpContext.User.Claims.Any())
                return HttpContext.User.Claims.First(f => f.Type.Equals(JwtRegisteredClaimNames.Name)).Value;

            return null;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUserEmail()
        {
            if (HttpContext.User.Claims.Any())
                return HttpContext.User.Claims.First(f => f.Type.Equals("contact")).Value;

            return null;
        }
    }
}
