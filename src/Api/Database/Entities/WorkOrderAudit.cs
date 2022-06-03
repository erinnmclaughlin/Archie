namespace Archie.Api.Database.Entities;

public class WorkOrderAudit : Audit
{
    public long WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }

    public class Configuration : Configuration<WorkOrderAudit>
    {
    }
}
