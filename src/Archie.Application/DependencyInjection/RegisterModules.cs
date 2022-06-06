using Archie.Application.Common;
using Archie.Application.Database;
using Archie.Application.Modules.Customers;
using Archie.Application.Modules.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Archie.Application.DependencyInjection;

public static class RegisterModules
{
    public static IServiceCollection AddApplicationModules(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ArchieContext>(options => options.UseSqlServer(config.GetConnectionString("ArchieDb")));
        services.AddScoped<IRepository, ArchieRepository>();

        services.AddControllers();

        // TODO: register these differently and more betterly
        services.AddScoped<ICurrentUserService, DumbCurrentUserService>();
        services.AddScoped<CreateCustomerModule.AuditFactory>();
        services.AddScoped<UpdateCustomerModule.AuditFactory>();
        services.AddScoped<CreateWorkOrderModule.AuditFactory>();

        return services;
    }
}
