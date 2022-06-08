using Archie.Shared.ValueObjects;

namespace Archie.Shared.Features.Customers.Update;

public class UpdateCustomerResponse
{
    public long Id { get; set; }
    public string CompanyName { get; set; }
    public Location Location { get; set; }

    public UpdateCustomerResponse(long id, string companyName, Location location)
    {
        Id = id;
        CompanyName = companyName;
        Location = location;
    }
}
