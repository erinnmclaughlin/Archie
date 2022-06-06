using Archie.Shared.Customers;
using Archie.Shared.Customers.Create;
using Archie.Shared.Customers.GetAll;
using Archie.Shared.Customers.GetAuditTrail;
using Archie.Shared.Customers.GetDetails;
using Archie.Shared.Customers.Update;
using Refit;

namespace Archie.WebUI.Clients;

[RefitClient]
public interface ICustomerClient
{
    [Post(CustomerEndpoints.CreateCustomer)]
    Task<IApiResponse<CreateCustomerResponse>> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);

    [Get(CustomerEndpoints.GetAllCustomers)]
    Task<IApiResponse<GetAllCustomersResponse>> GetAllAsync(CancellationToken ct = default);

    [Get(CustomerEndpoints.GetCustomerDetails)]
    Task<IApiResponse<GetCustomerDetailsResponse>> GetDetailsAsync(long id, CancellationToken ct = default);

    [Patch(CustomerEndpoints.UpdateCustomer)]
    Task<IApiResponse<UpdateCustomerResponse>> UpdateAsync(long id, UpdateCustomerRequest request, CancellationToken ct = default);

    [Get(CustomerEndpoints.GetAuditTrail)]
    Task<IApiResponse<GetCustomerAuditTrailResponse>> GetAuditTrailAsync(long id, CancellationToken ct = default);
}
