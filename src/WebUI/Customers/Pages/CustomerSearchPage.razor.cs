using Archie.Shared.Customers.Create;
using Archie.Shared.Customers.GetAll;
using Archie.WebUI.Customers.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Pages;

public partial class CustomerSearchPage
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;

    private GetAllCustomersResponse? Customers { get; set; }
    private CreateCustomerDialogLauncher DialogLauncher { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var response = await CustomerClient.GetAllAsync();
        Customers = response.Content;
    }

    private async Task LaunchCreateDialog()
    {
        var dialog = DialogLauncher.Show();
        var result = await dialog.Result;

        if (result.Data is CreateCustomerResponse response)
        {
            Customers?.Add(new GetAllCustomersResponse.CustomerDto(response.Id, response.CompanyName, response.Location));
        }
    }
}
