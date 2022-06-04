using Archie.Shared.ValueObjects;

namespace Archie.Shared.Customers.Create;

public class CreateCustomerResponse
{
    public long Id { get; set; }
    public string CompanyName { get; set; }
    public Location Location { get; set; }

    public CreateCustomerResponse(long id, string companyName, Location location)
    {
        Id = id;
        CompanyName = companyName;
        Location = location;
    }
}
