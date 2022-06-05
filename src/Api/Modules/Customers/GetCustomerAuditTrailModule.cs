using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Shared.Audits;
using Archie.Shared.Customers;
using Archie.Shared.Customers.GetAuditTrail;
using Ardalis.Specification;
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
        var spec = new Specification(id);
        var auditTrail = await Repository.ListAsync(spec, ct);
        return new GetCustomerAuditTrailResponse(auditTrail);
    }

    public class Specification : Specification<Audit, GetCustomerAuditTrailResponse.AuditDto>
    {
        public Specification(long customerId)
        {
            var eventTypes = new EventType[]
            {
                EventType.CustomerCreated,
                EventType.CustomerLocationUpdated,
                EventType.CustomerNameUpdated,
                EventType.WorkOrderCreated,
                EventType.WorkOrderCanceled,
                EventType.WorkOrderCompleted
            };

            Query
                .Select(a => new GetCustomerAuditTrailResponse.AuditDto
                (
                    a.AuditType,
                    a.EventType,
                    a.Description,
                    a.Timestamp,
                    new GetCustomerAuditTrailResponse.UserDto(a.User!.Id, a.User.FullName)
                )
                {
                    RelatedCustomers = a.Customers!.Select(c => new GetCustomerAuditTrailResponse.CustomerDto(c.Id, c.CompanyName)).ToList(),
                    RelatedWorkOrders = a.WorkOrders!.Select(wo => new GetCustomerAuditTrailResponse.WorkOrderDto(wo.Id, wo.ReferenceNumber)).ToList()
                })
                .Include(a => a.Customers!)
                .Include(a => a.WorkOrders!)
                .Include(a => a.User!)
                .Where
                (
                    a => eventTypes.Contains(a.EventType) && 
                    (
                        a.Customers!.Any(c => c.Id == customerId) || 
                        a.WorkOrders!.Any(wo => wo.CustomerId == customerId)
                    )
                )
                .OrderByDescending(a => a.Timestamp)
                .AsSplitQuery()
                .AsNoTracking();
        }
    }
}
