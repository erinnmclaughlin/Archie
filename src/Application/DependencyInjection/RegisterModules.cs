using Archie.Application.Common;
using Archie.Application.Database;
using Archie.Application.Modules.Customers;
using Archie.Application.Modules.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Archie.Application.DependencyInjection;

public static class RegisterModules
{
    public static IServiceCollection AddArchieModules<T>(this IServiceCollection services) where T : ICurrentUserService
    {
        services.AddScoped(typeof(ICurrentUserService), typeof(T));

        services.AddScoped<IRepository, ArchieRepository>();

        // TODO: register these differently and more betterly
        services.AddScoped<CreateCustomerModule.AuditFactory>();
        services.AddScoped<UpdateCustomerModule.AuditFactory>();
        services.AddScoped<CreateWorkOrderModule.AuditFactory>();

        return services;
    }

    public static IServiceCollection AddArchieSqlDb(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<ArchieContext>(options => options.UseSqlServer(connectionString));
    }
}
