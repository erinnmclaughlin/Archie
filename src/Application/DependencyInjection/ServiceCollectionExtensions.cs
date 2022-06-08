using Archie.Application.Common;
using Archie.Application.Database;
using Archie.Shared.Features.Customers.Create;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Archie.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddArchie<T>(this IServiceCollection services) where T : ICurrentUserService
    {
        services
            .AddScoped(typeof(ICurrentUserService), typeof(T))
            .AddScoped<IRepository, ArchieRepository>()
            .AddValidatorsFromAssemblyContaining<CreateCustomerRequest>();

        return services;
    }

    public static IServiceCollection AddArchieSqlDb(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<ArchieContext>(options => options.UseSqlServer(connectionString));
    }
}
