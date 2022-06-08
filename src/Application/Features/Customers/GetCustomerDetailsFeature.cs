using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Application.Exceptions;
using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.GetDetails;
using Archie.Shared.Features.WorkOrders;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.Customers;

[ApiController]
public class GetCustomerDetailsFeature : IFeature
{
    private IRepository Repository { get; }

    public GetCustomerDetailsFeature(IRepository repository)
    {
        Repository = repository;
    }

    [HttpGet(CustomerEndpoints.GetCustomerDetails)]
    public async Task<GetCustomerDetailsResponse> Update(long id, CancellationToken ct)
    {
        var spec = new Specification(id);
        return await Repository.FirstOrDefaultAsync(spec, ct) ?? throw new EntityNotFoundException("Customer not found.");
    }

    public class Specification : Specification<Customer, GetCustomerDetailsResponse>
    {
        public Specification(long id)
        {
            Query
                .Select(c => new GetCustomerDetailsResponse
                (
                    c.Id,
                    c.CompanyName,
                    c.Location
                )
                {
                    ActiveWorkOrders = c.WorkOrders!
                        .Where(wo => wo.Status != WorkOrderStatus.Canceled && wo.Status != WorkOrderStatus.Completed)
                        .Select(wo => new GetCustomerDetailsResponse.WorkOrderDto(wo.Id, wo.ReferenceNumber, wo.Status))
                        .ToList()
                })
                .Include(c => c.WorkOrders!)
                .Where(c => c.Id == id)
                .AsNoTracking();
        }
    }
}
