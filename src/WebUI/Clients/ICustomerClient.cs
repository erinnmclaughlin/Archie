using Archie.Shared.Modules.Customers;
using Archie.Shared.Modules.Customers.Create;
using Archie.Shared.Modules.Customers.GetAll;
using Archie.Shared.Modules.Customers.GetAuditTrail;
using Archie.Shared.Modules.Customers.GetDetails;
using Archie.Shared.Modules.Customers.Update;
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
