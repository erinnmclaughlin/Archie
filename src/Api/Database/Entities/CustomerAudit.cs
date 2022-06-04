using Archie.Shared.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public class CustomerAudit : Audit
{
    public CustomerEvent EventType { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public class Configuration : Configuration<CustomerAudit>
    {
        public override void Configure(EntityTypeBuilder<CustomerAudit> builder)
        {
            builder.Property(x => x.EventType).HasConversion<string>().HasMaxLength(50).HasColumnName("EventType");
            base.Configure(builder);
        }
    }
}
