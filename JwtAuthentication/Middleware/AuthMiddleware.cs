using JwtAuthentication.Services.AuthInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace JwtAuthentication.Middleware
{
    /// <summary>
    /// Middleware for consuming authentication data from JWT to auth service
    /// </summary>
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

            if (endpoint == default)
            {
                await _next(context);
                return;
            }

            bool isAuthorizeData = endpoint?.Metadata?.GetMetadata<IAuthorizeData>() != null;
            bool isAllowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

            if (isAuthorizeData && !isAllowAnonymous)
            {
                authService.Init(context.User);
            }

            await _next(context);
        }
    }
}
