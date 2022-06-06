using Archie.Shared.Modules.WorkOrders.GetAll;
using Archie.WebUI.Clients;
using Archie.WebUI.Features.WorkOrders.Dialogs;
using Archie.WebUI.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Features.WorkOrders.Pages;

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
