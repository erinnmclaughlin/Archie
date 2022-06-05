using Archie.Shared.WorkOrders.GetDetails;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.WorkOrders.Pages;

public partial class WorkOrderDetailsPage
{
    [Inject] private IWorkOrderClient WorkOrderClient { get; set; } = default!;
    [Parameter] public long Id { get; set; }

    private GetWorkOrderDetailsResponse? WorkOrder { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (WorkOrder?.Id == Id)
            return;

        var response = await WorkOrderClient.GetDetailsAsync(Id);
        WorkOrder = response.Content;
    }
}
