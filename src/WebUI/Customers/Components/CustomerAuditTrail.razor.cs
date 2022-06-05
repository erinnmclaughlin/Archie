using Archie.Shared.Audits;
using Archie.Shared.Customers.GetAuditTrail;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Customers.Components;

public partial class CustomerAuditTrail
{
    [Inject] private ICustomerClient CustomerClient { get; set; } = null!;
    [Parameter, EditorRequired] public long CustomerId { get; set; }

    private GetCustomerAuditTrailResponse? AuditTrail { get; set; }

    public async Task Refresh()
    {
        AuditTrail = null;
        StateHasChanged();

        await LoadAuditTrail();
    }

    protected override async Task OnParametersSetAsync()
    {
        await LoadAuditTrail();
    }

    private async Task LoadAuditTrail()
    {
        var response = await CustomerClient.GetAuditTrailAsync(CustomerId);
        AuditTrail = response.Content;
        StateHasChanged();
    }
}
