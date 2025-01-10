using JwtAuthentication;
using Persistence;
using Persistence.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);

// Our services
builder.Services.AddJwtAuthenticationWithIdentityDb<AppDbContext, ApplicationUser>(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// Our pipeline
app.UseJwtAuthentication<ApplicationUser>();
app.MapControllers();

await DbSeeder.SeedData(app);

app.Run();
