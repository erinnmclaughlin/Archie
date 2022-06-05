using Archie.Shared.WorkOrders;
using Archie.Shared.WorkOrders.Create;
using Archie.Shared.WorkOrders.GetAll;
using Refit;

namespace Archie.WebUI.WorkOrders;

public interface IWorkOrderClient
{
    [Post(WorkOrderEndpoints.CreateWorkOrder)]
    Task<IApiResponse<CreateWorkOrderResponse>> CreateAsync(CreateWorkOrderRequest request, CancellationToken ct = default);

    [Get(WorkOrderEndpoints.GetAllWorkOrders)]
    Task<IApiResponse<GetAllWorkOrdersResponse>> GetAllAsync(CancellationToken ct = default);
}
