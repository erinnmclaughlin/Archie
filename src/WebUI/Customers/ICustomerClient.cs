using Archie.Shared.Customers;
using Archie.Shared.Customers.Create;
using Refit;

namespace Archie.WebUI.Customers;

public interface ICustomerClient
{
    [Post(CustomerEndpoints.CreateCustomer)]
    Task<IApiResponse<CreateCustomerResponse>> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);
}
