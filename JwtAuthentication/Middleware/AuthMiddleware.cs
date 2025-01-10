﻿using JwtAuthentication.Services.AuthInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = context.User.FindFirstValue(ClaimTypes.Email);

                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(email))
                {
                    authService.Init(new Guid(userId!), email!);
                }
            }

            await _next(context);
        }
    }
}
