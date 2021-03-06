using Archie.Shared.Features.Customers.GetAll;
using Archie.WebUI.Clients;
using Archie.WebUI.Features.Customers.Dialogs;
using Archie.WebUI.Services.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Features.Customers.Pages.Search;

public partial class CustomerSearchPage
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    private GetAllCustomersResponse? Response { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await CustomerClient.GetAllAsync();
        Response = response.Content;
    }

    private async Task LaunchCreateDialog()
    {
        await DialogService.Show<CreateCustomerDialog>().Result;
    }
}
