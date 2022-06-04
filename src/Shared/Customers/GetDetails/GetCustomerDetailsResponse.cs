using Archie.Shared.ValueObjects;

namespace Archie.Shared.Customers.GetDetails;

public class GetCustomerDetailsResponse
{
    public long Id { get; set; }
    public string CompanyName { get; set; }
    public Location Location { get; set; }

    public GetCustomerDetailsResponse(long id, string companyName, Location location)
    {
        Id = id;
        CompanyName = companyName;
        Location = location;
    }
}
