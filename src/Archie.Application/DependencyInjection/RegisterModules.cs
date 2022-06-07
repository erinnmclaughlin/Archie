using Archie.Application.Common;
using Archie.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Archie.Application.DependencyInjection;

public static class RegisterModules
{
    public static IServiceCollection AddApplicationModules<T>(this IServiceCollection services, IConfiguration config) where T : ICurrentUserService
    {
        return services
            .AddDbContext<ArchieContext>(options => options.UseSqlServer(config.GetConnectionString("ArchieDb")))
            .AddScoped<IRepository, ArchieRepository>()
            .AddScoped(typeof(ICurrentUserService), typeof(T));
    }
}
