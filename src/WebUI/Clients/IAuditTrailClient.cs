using Archie.Shared.Modules.Audits;
using Archie.Shared.Modules.Audits.GetRecentActivity;
using Refit;

namespace Archie.WebUI.Clients;

[RefitClient]
public interface IAuditTrailClient
{
    [Get(AuditTrailEndpoints.GetRecentActivity)]
    Task<IApiResponse<GetRecentActivityResponse>> GetRecentActivityAsync(CancellationToken ct = default);
}
