﻿using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Modules.Audits;
using Archie.Shared.Modules.Audits.GetRecentActivity;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Modules.AuditTrails;

[ApiController]
public class GetRecentActivityModule : IModule
{
    private IRepository Repository { get; }

    public GetRecentActivityModule(IRepository repository)
    {
        Repository = repository;
    }

    [HttpGet(AuditTrailEndpoints.GetRecentActivity)]
    public async Task<GetRecentActivityResponse> GetRecentActivity(CancellationToken ct)
    {
        var spec = new Specification();
        var auditTrail = await Repository.ListAsync(spec, ct);
        return new GetRecentActivityResponse(auditTrail);
    }

    public class Specification : Specification<Audit, GetRecentActivityResponse.AuditDto>
    {
        public Specification()
        {
            Query
                .Select(a => new GetRecentActivityResponse.AuditDto
                (
                    a.AuditType,
                    a.EventType,
                    a.Description,
                    a.Timestamp,
                    new GetRecentActivityResponse.UserDto(a.User!.Id, a.User.FullName)
                )
                {
                    RelatedCustomers = a.Customers!.Select(c => new GetRecentActivityResponse.CustomerDto(c.Id, c.CompanyName)).ToList(),
                    RelatedWorkOrders = a.WorkOrders!.Select(wo => new GetRecentActivityResponse.WorkOrderDto(wo.Id, wo.ReferenceNumber)).ToList()
                })
                .Include(a => a.User!)
                .Include(a => a.Customers!)
                .Include(a => a.WorkOrders!)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .AsSplitQuery()
                .AsNoTracking();
        }
    }
}
