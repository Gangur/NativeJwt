using JwtAuthentication.Middleware;
using JwtAuthentication.Options;
using JwtAuthentication.Services.AuthInfo;
using JwtAuthentication.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JwtAuthentication
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddJwtAuthenticationWithIdentityDb<TContext, TUser>(this IServiceCollection serviceCollection, IConfiguration configuration)
            where TContext : IdentityDbContext<TUser>
            where TUser : IdentityUser
        {
            serviceCollection.ConfigureOptions<JwtAuthenticationOptionsSetup>();
            var jwtOptions = configuration
                .GetRequiredSection(JwtAuthenticationOptionsSetup.SectionName)
                .Get<JwtAuthenticationOptions>()!;

            bool debug =
#if DEBUG
                true;
#elif RELEASE
                false;
#endif

            serviceCollection
               .AddIdentity<TContext, IdentityRole>()
               .AddEntityFrameworkStores<TContext>()
               .AddDefaultTokenProviders();

            serviceCollection.AddSingleton<IJwtProvider, JwtProvider>();
            serviceCollection.AddScoped<IAuthService, AuthService>();

            serviceCollection
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = !debug;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidIssuer = jwtOptions.Issuer,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
                    };
                }
            );

            return serviceCollection;
        }

        public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<AuthMiddleware>();

            return applicationBuilder;
        }
    }
}
