﻿using JwtAuthentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthentication.Services.Jwt
{
    /// <summary>
    /// Implementation of JWT provider
    /// </summary>
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtAuthenticationOptions _options;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        public JwtProvider(IOptions<JwtAuthenticationOptions> options)
        {
            _options = options.Value;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.Secret));
        }

        /// <summary>
        /// Create new JWT for user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="roles">User roles</param>
        /// <returns>JSON Web Token</returns>
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

            return GenerateTokenByClaims(claims);
        }

        /// <summary>
        /// Try to refresh an expired token
        /// </summary>
        /// <param name="jwToken">An expired token</param>
        /// <returns>New token or null if the expired one is invalid</returns>
        public string? TryToRefresh(string jwToken)
        {
            var claimsPrincipal = GetPrincipalFromExpiredToken(jwToken);

            if (claimsPrincipal == default) 
            {
                return default;
            }

            var claims = claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c);

            claims.Remove("aud");

            return GenerateTokenByClaims(claims.Values);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private string GenerateTokenByClaims(IEnumerable<Claim> claims)
        {
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwToken"></param>
        /// <returns></returns>
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string jwToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _options.Issuer,
                ValidAudience = _options.Audience,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = _symmetricSecurityKey
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(jwToken, tokenValidationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512))
            {
                return default;
            }

            return principal;
        }
    }
}
