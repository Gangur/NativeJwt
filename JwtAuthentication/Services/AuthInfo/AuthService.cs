namespace JwtAuthentication.Services.AuthInfo
{
    public class AuthService : IAuthService
    {
        public int UserId { get => _userId; }
        public string Email { get => _email; }

        private int _userId;
        private string _email = string.Empty;

        public void Init(int userId, string email)
        {
            _userId = userId;
            _email = email;
        }
    }
}
