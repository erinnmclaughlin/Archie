using Archie.Shared.Modules.WorkOrders;
using Archie.Shared.Modules.WorkOrders.Create;
using Archie.Shared.Modules.WorkOrders.GetAll;
using Archie.Shared.Modules.WorkOrders.GetDetails;
using Refit;

namespace Archie.WebUI.Clients;

[RefitClient]
public interface IWorkOrderClient
{
    [Post(WorkOrderEndpoints.CreateWorkOrder)]
    Task<IApiResponse<CreateWorkOrderResponse>> CreateAsync(CreateWorkOrderRequest request, CancellationToken ct = default);

    [Get(WorkOrderEndpoints.GetAllWorkOrders)]
    Task<IApiResponse<GetAllWorkOrdersResponse>> GetAllAsync(CancellationToken ct = default);

    [Get(WorkOrderEndpoints.GetWorkOrderDetails)]
    Task<IApiResponse<GetWorkOrderDetailsResponse>> GetDetailsAsync(long id, CancellationToken ct = default);
}
