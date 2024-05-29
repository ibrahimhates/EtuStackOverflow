using AskForEtu.Core.Entity;
using AskForEtu.Core.JwtGenerator;
using AskForEtu.Core.Options;
using AskForEtu.Core.Services;
using AskForEtu.Core.Services.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AskForEtu.Repository.JwtGenerator
{
    public sealed class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;
        private readonly ITokenRepository _userTokenRepository;
        private readonly IUserRepository _userRepository;
        public JwtProvider(IOptions<JwtOptions> options,
            ITokenRepository userTokenRepository,
            IUserRepository userRepository)
        {
            _userTokenRepository = userTokenRepository;
            _options = options.Value;
            _userRepository=userRepository;
        }

        public string Generate(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim("usrId", user.Id.ToString()),
                new Claim("contact", user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = user.Roles.Select(x => x.Role).ToList();

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claims,
                null,
                DateTime.Now.AddHours(_options.Expires),
                signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return tokenValue;
        }

        public async Task<(bool, int?)> VerifyTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userIdValue = jwtToken.Claims.First(x => x.Type == "usrId").Value;
                int.TryParse(userIdValue, out var userId);

                var user = await _userRepository
                    .GetByCondition(u => u.Id == userId, false)
                    .Include(u => u.Token)
                    .FirstOrDefaultAsync();

                if (user is null) 
                    return (false, null);

                var userToken = user.Token;

                if (userToken is null) return (false, null);

                if(userToken.AccessToken.Equals(token)) return (false, null);    

                tokenHandler.ValidateToken(userToken.AccessToken, new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _options.Issuer,
                    ValidAudience = _options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_options.SecretKey))
                }, out SecurityToken validatedToken);

                if (token.Equals(userToken.AccessToken)) return (true, userId);

                return (false, null);
            }
            catch (Exception err)
            {
                return (false, null);
            }
        }
    }
}
