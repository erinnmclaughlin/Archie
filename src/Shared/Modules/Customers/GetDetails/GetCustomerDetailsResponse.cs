using Archie.Shared.Modules.WorkOrders;
using Archie.Shared.ValueObjects;

namespace Archie.Shared.Modules.Customers.GetDetails;

public class GetCustomerDetailsResponse
{
    public long Id { get; set; }
    public string CompanyName { get; set; }
    public Location Location { get; set; }

    public List<WorkOrderDto> ActiveWorkOrders { get; set; } = new();

    public GetCustomerDetailsResponse(long id, string companyName, Location location)
    {
        Id = id;
        CompanyName = companyName;
        Location = location;
    }

    public record WorkOrderDto(long Id, string ReferenceNumber, WorkOrderStatus Status);
}
