using Archie.Shared.ValueObjects;

namespace Archie.Shared.Features.WorkOrders.GetAll;

public class GetAllWorkOrdersResponse : List<GetAllWorkOrdersResponse.WorkOrderDto>
{
    public GetAllWorkOrdersResponse()
    {
    }

    public GetAllWorkOrdersResponse(IEnumerable<WorkOrderDto> workOrders)
    {
        AddRange(workOrders);
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

    public class WorkOrderDto
    { 
        public long Id { get; set; }
        public CustomerDto Customer { get; set; }
        public string ReferenceNumber { get; set; }
        public WorkOrderStatus Status { get; set; }

        public WorkOrderDto(long id, CustomerDto customer, string referenceNumber, WorkOrderStatus status)
        {
            Id = id;
            Customer = customer;
            ReferenceNumber = referenceNumber;
            Status = status;
        }
    }
}
