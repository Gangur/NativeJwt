using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddPersistence(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("DefaultConnection");

            serviceCollection.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));

            return serviceCollection;
        }
    }
}
