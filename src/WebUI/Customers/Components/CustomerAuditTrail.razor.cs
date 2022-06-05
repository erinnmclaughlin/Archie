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

    private static string GetIcon(AuditType auditType, EventType eventType)
    {
        if (auditType == AuditType.Create)
        {
            return "text-success " + eventType switch
            {
                EventType.CustomerCreated => "fa-solid fa-user-plus",
                EventType.WorkOrderCreated => "fa-solid fa-file-circle-plus",
                _ => "fa-solid fa-plus"
            };
        }

        if (auditType == AuditType.Update)
        {
            return "text-primary " + eventType switch
            {
                EventType.CustomerLocationUpdated => "fa-solid fa-earth-americas",
                EventType.CustomerNameUpdated => "fa-solid fa-user-tag",
                _ => "fa-solid fa-pencil-alt"
            };
        }

        if (auditType == AuditType.Delete)
        {
            return "text-danger " + eventType switch
            {
                _ => "fa-solid fa-trash-alt"
            };
        }

        return "fa-solid fa-plus text-muted";
    }
}
