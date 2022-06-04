using Archie.Shared.ValueObjects;

namespace Archie.Shared.Customers.GetAll;

public class GetAllCustomersResponse : List<GetAllCustomersResponse.CustomerDto>
{
    public GetAllCustomersResponse()
    {
    }

    public GetAllCustomersResponse(IEnumerable<CustomerDto> customers)
    {
        AddRange(customers);
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
