using JwtAuthentication.Services.AuthInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuthentication.Middleware
{
    internal class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
            IAuthService authService)
        {
            var endpoint = context.GetEndpoint();
            bool isAllowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

            if (!isAllowAnonymous)
            {
                var userId = context.User.FindFirstValue(JwtRegisteredClaimNames.NameId);
                var email = context.User.FindFirstValue(JwtRegisteredClaimNames.Email);

                authService.Init(Convert.ToInt32(userId), email!);
            }

            await _next(context);
        }
    }
}
