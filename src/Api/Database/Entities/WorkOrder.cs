namespace Archie.Api.Database.Entities;

public class WorkOrder
{
    public long Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;

    public long CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<WorkOrderAudit>? AuditTrail { get; set; }
}
