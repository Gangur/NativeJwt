using Microsoft.AspNetCore.Identity;

namespace JwtAuthentication.Services.Jwt
{
    public interface IJwtProvider
    {
        string Generate(IdentityUser user, string[] roles);

        string? TryToRefrash(string jwToken);
    }
}
