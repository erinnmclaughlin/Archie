using Archie.WebUI.Customers.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Pages;

public partial class CustomerSearchPage
{
    [Inject] private CreateCustomerDialog CreateCustomerDialog { get; set; } = null!;

    private async Task LaunchCreateDialog()
    {
        var dialog = CreateCustomerDialog.Show();
        await dialog.Result;
    }
}
