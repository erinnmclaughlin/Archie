using Archie.Shared.WorkOrders.GetAll;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.WorkOrders.Pages;

public partial class WorkOrderSearchPage
{
    [Inject] private IWorkOrderClient WorkOrderClient { get; set; } = default!;

    private GetAllWorkOrdersResponse? WorkOrders { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await WorkOrderClient.GetAllAsync();
        WorkOrders = response.Content;
    }
}
