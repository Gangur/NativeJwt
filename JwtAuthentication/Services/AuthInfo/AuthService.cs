using System.Security.Claims;

namespace JwtAuthentication.Services.AuthInfo
{
    /// <summary>
    /// Authentication service basic implimintation
    /// Can be replaced in DI
    /// </summary>
    public class AuthService : IAuthService
    {
        public Guid UserId { get => _userId; }
        public string Email { get => _email; }

        private Guid _userId;
        private string _email = string.Empty;

        public void Init(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            _userId = new Guid(userId!);
            _email = email!;
        }
    }
}
