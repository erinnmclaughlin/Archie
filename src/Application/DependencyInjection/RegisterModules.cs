using Archie.Application.Common;
using Archie.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Archie.Application.DependencyInjection;

public static class RegisterModules
{
    public static IServiceCollection AddArchieModules<T>(this IServiceCollection services) where T : ICurrentUserService
    {
        services.AddScoped(typeof(ICurrentUserService), typeof(T));

        services.AddScoped<IRepository, ArchieRepository>();
        return services;
    }

    public static IServiceCollection AddArchieSqlDb(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<ArchieContext>(options => options.UseSqlServer(connectionString));
    }
}
