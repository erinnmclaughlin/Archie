using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.Create;
using Archie.Shared.Features.Customers.GetAll;
using Archie.Shared.Features.Customers.GetAuditTrail;
using Archie.Shared.Features.Customers.GetDetails;
using Archie.Shared.Features.Customers.Update;
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
