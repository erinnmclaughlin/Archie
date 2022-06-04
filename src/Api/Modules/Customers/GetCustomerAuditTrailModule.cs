using Archie.Api.Common;
using Archie.Shared.Customers;
using Archie.Shared.Customers.GetAuditTrail;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.Customers;

[ApiController]
public class GetCustomerAuditTrailModule : IModule
{
    private IRepository Repository { get; }

    public GetCustomerAuditTrailModule(IRepository repository)
    {
        Repository = repository;
    }

    [HttpGet(CustomerEndpoints.GetAuditTrail)]
    public async Task<GetCustomerAuditTrailResponse> GetAuditTrail(long id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

}
