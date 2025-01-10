using JwtAuthentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthentication.Services.Jwt
{
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtAuthenticationOptions _options;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        public JwtProvider(IOptions<JwtAuthenticationOptions> options)
        {
            _options = options.Value;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Secret));
        }

        public string Generate(IdentityUser user, string[] roles)
        {
            var claims = new List<Claim>(3 + roles.Length)
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName!)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_options.ExpirationPeriod),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
