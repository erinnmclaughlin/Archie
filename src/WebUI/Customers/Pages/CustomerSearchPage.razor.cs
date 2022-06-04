using Archie.Shared.Customers.Create;
using Archie.Shared.Customers.GetAll;
using Archie.WebUI.Customers.Dialogs;
using Archie.WebUI.Shared.Dialogs;
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
        var dialog = DialogService.Show<CreateCustomerDialog>();
        var result = await dialog.Result;

        if (result.Data is CreateCustomerResponse response)
        {
            Customers?.Add(new GetAllCustomersResponse.CustomerDto(response.Id, response.CompanyName, response.Location));
        }
    }
}
