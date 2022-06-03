using Archie.Api.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Archie.Api.Database;

public class ArchieContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<CustomerAudit> CustomerAudits => Set<CustomerAudit>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderAudit> WorkOrderAudits => Set<WorkOrderAudit>();

    public ArchieContext(DbContextOptions<ArchieContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
