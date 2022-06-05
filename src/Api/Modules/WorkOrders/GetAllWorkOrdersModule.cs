using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Shared.WorkOrders;
using Archie.Shared.WorkOrders.GetAll;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.WorkOrders;

[ApiController]
public class GetAllWorkOrdersModule : IModule
{
    private IRepository Repository { get; }

    public GetAllWorkOrdersModule(IRepository repository)
    {
        Repository = repository;
    }

    [HttpGet(WorkOrderEndpoints.GetAllWorkOrders)]
    public async Task<GetAllWorkOrdersResponse> GetAllWorkOrders(CancellationToken ct)
    {
        var spec = new Specification();
        var workOrders = await Repository.ListAsync(spec, ct);
        return new GetAllWorkOrdersResponse(workOrders);
    }

    public class Specification : Specification<WorkOrder, GetAllWorkOrdersResponse.WorkOrderDto>
    {
        public Specification()
        {
            Query
                .Select(wo => new GetAllWorkOrdersResponse.WorkOrderDto
                (
                    wo.Id,
                    new GetAllWorkOrdersResponse.CustomerDto
                    (
                        wo.Customer!.Id,
                        wo.Customer.CompanyName,
                        wo.Customer.Location
                    ),
                    wo.ReferenceNumber,
                    wo.Status
                ))
                .Include(wo => wo.Customer!)
                .AsNoTracking();
        }
    }
}
