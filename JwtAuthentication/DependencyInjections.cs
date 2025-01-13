using JwtAuthentication.Middleware;
using JwtAuthentication.Options;
using JwtAuthentication.Services.AuthInfo;
using JwtAuthentication.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;

namespace JwtAuthentication
{
    /// <summary>
    /// Dependency injections for JWT services registration
    /// </summary>
    public static class DependencyInjections
    {
        /// <summary>
        /// Add JWT Authentication based on AspNetCore Identity provider
        /// </summary>
        /// <typeparam name="TContext">EF context, which implements IdentityDbContext</typeparam>
        /// <typeparam name="TUser">User entity, which implements IdentityUser</typeparam>
        /// <param name="serviceCollection">Service Collection</param>
        /// <param name="configuration">App Configuration</param>
        /// <returns>Service Collection</returns>
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
               .AddIdentity<TUser, IdentityRole>()
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

        /// <summary>
        /// Add JWT Middleware and endpoints
        /// </summary>
        /// <typeparam name="TUser">User entity, which implements IdentityUser</typeparam>
        /// <param name="applicationBuilder">Application Builder</param>
        /// <returns>Application Builder</returns>
        public static IApplicationBuilder UseJwtAuthentication<TUser>(this IApplicationBuilder applicationBuilder)
            where TUser : IdentityUser
        {
            applicationBuilder.UseMiddleware<AuthMiddleware>();

            const string authRoute = "jwt-auth";

            applicationBuilder.UseEndpoints(endpoints => // Here can be basic endpoints like login 
            {
                endpoints.MapGet($"{authRoute}/get-token", async context =>
                {
                    var userManager = context.RequestServices.GetRequiredService<UserManager<TUser>>();
                    var jwtProvider = context.RequestServices.GetRequiredService<IJwtProvider>();

                    var admin = await userManager.FindByEmailAsync("admin@gmail.com") ?? throw new NullReferenceException(); // For sample purpose
                    var roles = await userManager.GetRolesAsync(admin);
                    var token = jwtProvider.Generate(admin, roles.ToArray());

                    await context.Response.WriteAsync(token);
                });
                endpoints.MapGet($"{authRoute}/refrash-token", async context =>
                {
                    var jwtProvider = context.RequestServices.GetRequiredService<IJwtProvider>();
                    var jwtHeader = context.Request.Headers["Authorization"].ToString();

                    if (string.IsNullOrEmpty(jwtHeader))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("The authorization header hasn't been found!");
                        return;
                    }

                    var iwtSplitArray = jwtHeader.Split(' ');

                    if (iwtSplitArray.Length != 2)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("The authorization header does not have the valid structure!");
                        return;
                    }

                    var newJwt = jwtProvider.TryToRefresh(iwtSplitArray.Last());

                    if (string.IsNullOrEmpty(newJwt))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync("An invalid token has been provided!");
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        await context.Response.WriteAsync(newJwt);
                    }
                });
            });

            return applicationBuilder;
        }
    }
}
