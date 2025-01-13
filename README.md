## JwtAuthentication - Library project

The key implementation in [JwtAuthentication/DependencyInjections.cs](https://github.com/Gangur/NativeJwt/blob/master/JwtAuthentication/DependencyInjections.cs):
```C#
public static IServiceCollection AddJwtAuthenticationWithIdentityDb<TContext, TUser>(this IServiceCollection serviceCollection, IConfiguration configuration)
    where TContext : IdentityDbContext<TUser>
    where TUser : IdentityUser

public static IApplicationBuilder UseJwtAuthentication<TUser>(this IApplicationBuilder applicationBuilder)
    where TUser : IdentityUser
```

To request and refresh the token, there are two built-in endpoints:
```C#
public static IApplicationBuilder UseJwtAuthentication<TUser>(this IApplicationBuilder applicationBuilder)
    where TUser : IdentityUser
{
    ...

    const string authRoute = "jwt-auth";

    applicationBuilder.UseEndpoints(endpoints => // Here can be basic endpoints like login 
    {
        endpoints.MapGet($"{authRoute}/get-token", async context =>
        {
            ...
        });
        endpoints.MapGet($"{authRoute}/refrash-token", async context =>
        {
           ...
        });
    });
    ...
}
```

## NativeJwt - Web Api project

To register JWT services in [NativeJwt/Program.cs](https://github.com/Gangur/NativeJwt/blob/master/NativeJwt/Program.cs) (or Startup.cs):
```C#
// Register EF IdentityDbContext<TUser> where TUser : IdentityUser
builder.Services.AddPersistence(builder.Configuration);

// Register our JWT services
builder.Services.AddJwtAuthenticationWithIdentityDb<AppDbContext, ApplicationUser>(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// Our pipeline
app.UseJwtAuthentication<ApplicationUser>();
```
> ATTENTION: The order is crucially important

## Persistence - Domain and EF project
This is a project with Entity Framework and its IdentityDbContext.
See [Persistence/AppDbContext.cs](https://github.com/Gangur/NativeJwt/blob/master/Persistence/AppDbContext.cs)
