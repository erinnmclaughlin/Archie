using Archie.Shared.Audits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public class WorkOrderAudit : Audit
{
    public WorkOrderEvent EventType { get; set; }

    public long WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }

    public class Configuration : Configuration<WorkOrderAudit>
    {
        public override void Configure(EntityTypeBuilder<WorkOrderAudit> builder)
        {
            builder.Property(x => x.EventType).HasConversion<string>().HasMaxLength(50).HasColumnName("EventType");
            base.Configure(builder);
        }
    }
}
