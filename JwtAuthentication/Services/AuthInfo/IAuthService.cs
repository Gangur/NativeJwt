namespace JwtAuthentication.Services.AuthInfo
{
    public interface IAuthService
    {
        public Guid UserId { get; }
        public string Email { get; }
        public void Init(Guid userId, string email);
    }
}
