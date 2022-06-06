using Archie.Shared.Audits;

namespace Archie.Shared.Customers.GetAuditTrail;

public record GetCustomerAuditTrailResponse(List<GetCustomerAuditTrailResponse.AuditDto> Audits)
{
    public record AuditDto(AuditType AuditType, EventType EventType, string Description, DateTime Timestamp, UserDto User)
    {
        public List<CustomerDto> RelatedCustomers { get; set; } = new();
        public List<WorkOrderDto> RelatedWorkOrders { get; set; } = new();
    }

    public record CustomerDto(long Id, string CompanyName);
    public record UserDto(long Id, string FullName);
    public record WorkOrderDto(long Id, string ReferenceNumber);
}
