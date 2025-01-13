using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace JwtAuthentication.Options
{
    /// <summary>
    /// Setup for JWT authentication options
    /// </summary>
    internal class JwtAuthenticationOptionsSetup : IConfigureOptions<JwtAuthenticationOptions>
    {
        public const string SectionName = "JwtAuthentication";
        private readonly IConfiguration _configuration;

        public JwtAuthenticationOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtAuthenticationOptions options)
            => _configuration.GetSection(SectionName).Bind(options);
    }
}
