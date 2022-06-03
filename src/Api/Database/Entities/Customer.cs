using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public class Customer
{
    public long Id { get; set; }
    public string CompanyName { get; set; } = null!;
    public string? City { get; set; }
    public string? Region { get; set; }
    public string Country { get; set; } = null!;

    public ICollection<CustomerAudit>? AuditTrail { get; set; }

    public class Configuration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasIndex(x => x.CompanyName);

            builder.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.City).HasMaxLength(100).HasColumnName("City");
            builder.Property(x => x.Region).HasMaxLength(100).HasColumnName("Region");
            builder.Property(x => x.Country).HasMaxLength(100).HasColumnName("Country");
        }
    }
}
