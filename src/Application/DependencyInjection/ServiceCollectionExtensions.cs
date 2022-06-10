using Archie.Application.Common;
using Archie.Application.Database;
using Archie.Application.Features.Customers.Create;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Archie.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddArchie<T>(this IServiceCollection services) where T : ICurrentUserService
    {
        services
            .AddScoped(typeof(ICurrentUserService), typeof(T))
            .AddScoped<IRepository, ArchieRepository>()
            .AddScoped<ICreateCustomerAuditService, CreateCustomerAuditService>()
            .AddAutoMapper(Assembly.GetExecutingAssembly())
            .AddValidatorsFromAssembly(Assembly.Load("Archie.Shared"));

        return services;
    }

    public static IServiceCollection AddArchieSqlDb(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<ArchieContext>(options => options.UseSqlServer(connectionString));
    }
}
