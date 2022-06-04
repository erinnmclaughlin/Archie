using Archie.Shared.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public class Audit
{
    public long Id { get; set; }
    public AuditType AuditType { get; set; }
    public string Description { get; set; } = string.Empty;
    public EventType EventType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public long UserId { get; set; }
    public User? User { get; set; }

    public ICollection<Customer>? Customers { get; set; }
    public ICollection<WorkOrder>? WorkOrders { get; set; }

    public abstract class Configuration<TAudit> : IEntityTypeConfiguration<TAudit> where TAudit : Audit
    {
        public virtual void Configure(EntityTypeBuilder<TAudit> builder)
        {
            builder.HasIndex(x => x.AuditType);

            builder.HasOne(x => x.User).WithMany().OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.AuditType).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
            builder.Property(x => x.EventType).HasConversion<string>().HasMaxLength(50);
        }
    }
}
