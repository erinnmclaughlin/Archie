using Archie.Shared.Audits;

namespace Archie.Shared.Customers.GetAuditTrail;

public class GetCustomerAuditTrailResponse : List<GetCustomerAuditTrailResponse.AuditDto>
{
    public GetCustomerAuditTrailResponse()
    {
    }

    public GetCustomerAuditTrailResponse(IEnumerable<AuditDto> audits)
    {
        AddRange(audits);
    }

    public record AuditDto(AuditType AuditType, EventType EventType, string Description, DateTime Timestamp, UserDto User);
    public record UserDto(long Id, string FullName);
}
