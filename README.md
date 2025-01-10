## JwtAuthentication - Library location

Key implementation in [JwtAuthentication/DependencyInjections.cs](https://github.com/Gangur/NativeJwt/blob/master/JwtAuthentication/DependencyInjections.cs):
```C#
public static IServiceCollection AddJwtAuthenticationWithIdentityDb<TContext, TUser>(this IServiceCollection serviceCollection, IConfiguration configuration)
    where TContext : IdentityDbContext<TUser>
    where TUser : IdentityUser

public static IApplicationBuilder UseJwtAuthentication<TUser>(this IApplicationBuilder applicationBuilder)
    where TUser : IdentityUser
```

## NativeJwt - Web Api location

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

## Persistence - Domain and EF location
This is a project with Entity Framework and its IdentityDbContext.
See [Persistence/AppDbContext.cs](https://github.com/Gangur/NativeJwt/blob/master/Persistence/AppDbContext.cs)
