using Archie.Shared.WorkOrders.GetAll;
using Archie.WebUI.Services.Dialogs;
using Archie.WebUI.WorkOrders.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.WorkOrders.Pages;

public partial class WorkOrderSearchPage
{
    [Inject] private IDialogService DialogService { get; set; } = default!;
    [Inject] private IWorkOrderClient WorkOrderClient { get; set; } = default!;

    private GetAllWorkOrdersResponse? WorkOrders { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await WorkOrderClient.GetAllAsync();
        WorkOrders = response.Content;
    }

    private async Task LaunchCreateDialog()
    {
        await DialogService.Show<CreateWorkOrderDialog>().Result;
    }

}
