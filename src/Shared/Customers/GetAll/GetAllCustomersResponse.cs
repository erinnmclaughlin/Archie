using Archie.Shared.ValueObjects;

namespace Archie.Shared.Customers.GetAll;

public record GetAllCustomersResponse(List<GetAllCustomersResponse.CustomerDto> Customers)
{
    public record CustomerDto(long id, string companyName, Location location);
}
