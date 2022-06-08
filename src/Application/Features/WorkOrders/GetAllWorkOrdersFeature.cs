using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.WorkOrders;
using Archie.Shared.Features.WorkOrders.GetAll;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.WorkOrders;

[ApiController]
public class GetAllWorkOrdersFeature : IFeature
{
    private IRepository Repository { get; }

    public GetAllWorkOrdersFeature(IRepository repository)
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
