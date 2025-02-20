using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Travl.Domain.Context;

namespace Travl.Api.Extensions
{
    public static class DBRegistryExtension
    {
        public static void AddDbContextAndConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<ApplicationContext>(options =>
            {
                var dbProvider = configuration.GetValue<string>("DatabaseProvider")?.ToLower();

                if (dbProvider == "postgres")
                {
                    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                }
                else if (dbProvider == "sqlserver")
                {
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                }
                else
                {
                    throw new Exception("DatabaseProvider must be specified as 'postgres' or 'sqlserver'.");
                }
            });
        }
    }
}
