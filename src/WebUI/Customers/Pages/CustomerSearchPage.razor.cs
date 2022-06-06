using Archie.Shared.Customers.GetAll;
using Archie.WebUI.Clients;
using Archie.WebUI.Customers.Dialogs;
using Archie.WebUI.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Pages;

public partial class CustomerSearchPage
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    private GetAllCustomersResponse? Customers { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await CustomerClient.GetAllAsync();
        Customers = response.Content;
    }

    private async Task LaunchCreateDialog()
    {
        await DialogService.Show<CreateCustomerDialog>().Result;
    }
}
