using Archie.Shared.ValueObjects;
using Archie.Shared.WorkOrders;

namespace Archie.Shared.Customers.GetDetails;

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
