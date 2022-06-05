using Archie.Api.Common;
using Archie.Api.Modules.Customers;
using Archie.Api.Modules.WorkOrders;
using System.Reflection;

namespace Archie.Api.DependencyInjection;

public static class RegisterModules
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services)
    {
        var modules = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && typeof(IModule).IsAssignableFrom(t));

        foreach (var module in modules)
            services.AddScoped(module);

        // TODO: register these differently and more betterly
        services.AddScoped<ICurrentUserService, DumbCurrentUserService>();
        services.AddScoped<CreateCustomerModule.AuditFactory>();
        services.AddScoped<UpdateCustomerModule.AuditFactory>();
        services.AddScoped<CreateWorkOrderModule.AuditFactory>();

        return services;
    }
}
