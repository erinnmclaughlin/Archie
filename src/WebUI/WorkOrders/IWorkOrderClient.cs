using Archie.Shared.WorkOrders;
using Archie.Shared.WorkOrders.GetAll;
using Refit;

namespace Archie.WebUI.WorkOrders;

public interface IWorkOrderClient
{
    [Get(WorkOrderEndpoints.GetAllWorkOrders)]
    Task<IApiResponse<GetAllWorkOrdersResponse>> GetAllAsync(CancellationToken ct = default);
}
