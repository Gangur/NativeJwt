namespace JwtAuthentication.Services.AuthInfo
{
    public interface IAuthService
    {
        public int UserId { get; }
        public string Email { get; }
        public void Init(int userId, string email);
    }
}
