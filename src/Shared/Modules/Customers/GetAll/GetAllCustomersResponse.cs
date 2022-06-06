using Archie.Shared.ValueObjects;

namespace Archie.Shared.Modules.Customers.GetAll;

public record GetAllCustomersResponse(List<GetAllCustomersResponse.CustomerDto> Customers)
{
    public record CustomerDto(long Id, string CompanyName, Location Location);
}
