using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AskForEtu.Core.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EtuStackOverflow.OptionsSetup
{
    public class JwtBearerOptionsSetup : IConfigureOptions<JwtBearerOptions>
    {
        private readonly JwtOptions _jwtOptions;

        public JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions=jwtOptions.Value;
        }

        public void Configure(JwtBearerOptions options)
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
            };
        }
    }
}
