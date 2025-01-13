using System.Security.Claims;

namespace JwtAuthentication.Services.AuthInfo
{
    /// <summary>
    /// Authentication service abstraction
    /// Use ServiceDescriptor to replase AuthService
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// User Identity
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Initialization method
        /// </summary>
        /// <param name="claimsPrincipal">Claims from JWT</param>
        public void Init(ClaimsPrincipal claimsPrincipal);
    }
}
