using Archie.Shared.Audits;
using Archie.Shared.Audits.GetRecentActivity;
using Refit;

namespace Archie.WebUI.AuditTrails;

public interface IAuditTrailClient
{
    [Get(AuditTrailEndpoints.GetRecentActivity)]
    Task<IApiResponse<GetRecentActivityResponse>> GetRecentActivityAsync(CancellationToken ct = default);
}
