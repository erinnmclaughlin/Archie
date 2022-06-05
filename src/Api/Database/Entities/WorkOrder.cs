using Archie.Shared.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public class WorkOrder
{
    public long Id { get; set; }
    public DateTime? DueDate { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public WorkOrderStatus Status { get; set; }

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<Audit>? AuditTrail { get; set; }

    public class Configuration : IEntityTypeConfiguration<WorkOrder>
    {
        public void Configure(EntityTypeBuilder<WorkOrder> builder)
        {
            builder.HasIndex(x => x.ReferenceNumber).IsUnique();
            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Customer).WithMany().OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.ReferenceNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50).IsRequired();
        }
    }
}
