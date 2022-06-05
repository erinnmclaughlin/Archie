﻿using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Api.Exceptions;
using Archie.Shared.WorkOrders;
using Archie.Shared.WorkOrders.GetDetails;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.WorkOrders;

[ApiController]
public class GetWorkOrderDetailsModule : IModule
{
    private IRepository Repository { get; }

    public GetWorkOrderDetailsModule(IRepository repository)
    {
        Repository = repository;
    }

    [HttpGet(WorkOrderEndpoints.GetWorkOrderDetails)]
    public async Task<GetWorkOrderDetailsResponse> GetDetails(long id, CancellationToken ct)
    {
        var spec = new Specification(id);
        return await Repository.FirstOrDefaultAsync(spec, ct)
            ?? throw new EntityNotFoundException("Work order not found.");
    }

    public class Specification : Specification<WorkOrder, GetWorkOrderDetailsResponse>
    {
        public Specification(long id)
        {
            Query
                .Select(wo => new GetWorkOrderDetailsResponse
                (
                    wo.Id,
                    new GetWorkOrderDetailsResponse.CustomerDto
                    (
                        wo.Customer!.Id,
                        wo.Customer.CompanyName,
                        wo.Customer.Location
                    ),
                    wo.ReferenceNumber,
                    wo.Status
                ))
                .Include(wo => wo.Customer!)
                .Where(wo => wo.Id == id)
                .AsNoTracking();
        }
    }
}
