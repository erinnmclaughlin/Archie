using Archie.Shared.Customers;
using Archie.Shared.Customers.Create;
using Archie.Shared.Customers.GetAll;
using Refit;

namespace Archie.WebUI.Customers;

public interface ICustomerClient
{
    [Post(CustomerEndpoints.CreateCustomer)]
    Task<IApiResponse<CreateCustomerResponse>> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);

    [Get(CustomerEndpoints.GetAllCustomers)]
    Task<IApiResponse<GetAllCustomersResponse>> GetAllAsync(CancellationToken ct = default);
}
