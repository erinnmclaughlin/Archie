using Archie.Shared.Features.Audits.GetRecentActivity;
using Archie.WebUI.Clients;
using Microsoft.AspNetCore.Components;

namespace Archie.WebUI.Features.AuditTrails.Components;

public partial class RecentActivity
{
    [Inject] private IAuditTrailClient ApiClient { get; set; } = default!;

    private GetRecentActivityResponse? Activity { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiClient.GetRecentActivityAsync();
        Activity = response.Content;
    }
}
