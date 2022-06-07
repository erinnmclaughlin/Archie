using Archie.Application.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Archie.Application.Database;

public class ArchieContext : DbContext
{
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    public ArchieContext(DbContextOptions<ArchieContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
