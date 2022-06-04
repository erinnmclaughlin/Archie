using Archie.Shared.Customers.GetDetails;
using Archie.Shared.Customers.Update;
using Archie.WebUI.Customers.Components;
using Archie.WebUI.Customers.Dialogs;
using Archie.WebUI.Shared.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Pages;

public partial class CustomerDetailsPage
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = default!;
    [Inject] private IDialogService DialogService { get; set; } = default!;

    [Parameter] public long Id { get; set; }

    private CustomerAuditTrail? AuditTrail { get; set; }
    private GetCustomerDetailsResponse? Customer { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (Customer?.Id == Id)
            return;

        var response = await CustomerClient.GetDetailsAsync(Id);
        Customer = response.Content;
    }

    private async Task LaunchUpdateDialog()
    {
        if (Customer == null)
            throw new InvalidOperationException("Customer must be loaded to perform this action.");

        var parameters = new UpdateCustomerDialog.DialogParameters(Customer.Id, Customer.CompanyName, Customer.Location);
        var result = await DialogService.Show<UpdateCustomerDialog, UpdateCustomerDialog.DialogParameters>(parameters).Result;

        if (result.Data is UpdateCustomerResponse newCustomer)
        {
            Customer.CompanyName = newCustomer.CompanyName;
            Customer.Location = newCustomer.Location;

            AuditTrail?.Refresh();
        }
    }
}
