using Archie.Shared.ValueObjects;

namespace Archie.Shared.Features.WorkOrders.GetDetails;

public class GetWorkOrderDetailsResponse
{
    public long Id { get; set; }
    public CustomerDto Customer { get; set; }
    public string ReferenceNumber { get; set; }
    public WorkOrderStatus Status { get; set; }

    public GetWorkOrderDetailsResponse(long id, CustomerDto customer, string referenceNumber, WorkOrderStatus status)
    {
        Id = id;
        Customer = customer;
        ReferenceNumber = referenceNumber;
        Status = status;
    }

    public class CustomerDto
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public Location Location { get; set; }

        public CustomerDto(long id, string companyName, Location location)
        {
            Id = id;
            CompanyName = companyName;
            Location = location;
        }
    }
}
