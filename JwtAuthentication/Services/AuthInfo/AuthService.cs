namespace JwtAuthentication.Services.AuthInfo
{
    public class AuthService : IAuthService
    {
        public Guid UserId { get => _userId; }
        public string Email { get => _email; }

        private Guid _userId;
        private string _email = string.Empty;

        public void Init(Guid userId, string email)
        {
            _userId = userId;
            _email = email;
        }
    }
}
