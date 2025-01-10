using JwtAuthentication.Services.AuthInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NativeJwt.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthInfoController : Controller
    {
        private readonly IAuthService _authService;
        public AuthInfoController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Index()
            => Ok($"{_authService.UserId} - {_authService.Email}");
    }
}
