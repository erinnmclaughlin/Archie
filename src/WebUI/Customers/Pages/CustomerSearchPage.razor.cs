using Archie.WebUI.Customers.Dialogs;

namespace Archie.WebUI.Customers.Pages;

public partial class CustomerSearchPage
{
    private CreateCustomerDialogLauncher DialogLauncher { get; set; } = null!;

    private async Task LaunchCreateDialog()
    {
        var dialog = DialogLauncher.Show();
        await dialog.Result;
    }
}
