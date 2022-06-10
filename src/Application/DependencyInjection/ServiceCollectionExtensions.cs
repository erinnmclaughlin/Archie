using Archie.Application.Common;
using Archie.Application.Database;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Archie.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddArchie<T>(this IServiceCollection services) where T : ICurrentUserService
    {
        return services
            .AddScoped(typeof(ICurrentUserService), typeof(T))
            .AddScoped<IRepository, ArchieRepository>()
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidatorsFromAssembly(Assembly.Load("Archie.Shared"));
    }

    public static IServiceCollection AddArchieSqlDb(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<ArchieContext>(options => options.UseSqlServer(connectionString));
    }
}
