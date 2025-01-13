using Microsoft.AspNetCore.Identity;

namespace JwtAuthentication.Services.Jwt
{
    /// <summary>
    /// JWT provider abstraction
    /// </summary>
    public interface IJwtProvider
    {
        /// <summary>
        /// Create new JWT for user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="roles">User roles</param>
        /// <returns>JSON Web Token</returns>
        string Generate(IdentityUser user, string[] roles);

        /// <summary>
        /// Try to refresh an expired token
        /// </summary>
        /// <param name="jwToken">An expired token</param>
        /// <returns>New token or null if the expired one is invalid</returns>
        string? TryToRefresh(string jwToken);
    }
}
