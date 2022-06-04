using Archie.Shared.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public class Customer
{
    public long Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public Location Location { get; set; } = new();

    public ICollection<CustomerAudit>? AuditTrail { get; set; }

    public class Configuration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasIndex(x => x.CompanyName);

            builder.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();

            builder.OwnsOne(x => x.Location, location =>
            {
                location.Property(x => x.City).HasMaxLength(100).HasColumnName("City");
                location.Property(x => x.Region).HasMaxLength(100).HasColumnName("Region");
                location.Property(x => x.Country).HasMaxLength(100).HasColumnName("Country");
            });
        }
    }
}
