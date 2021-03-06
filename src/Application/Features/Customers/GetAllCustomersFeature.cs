using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.GetAll;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.Customers;

[ApiController]
public class GetAllCustomersFeature : IFeature
{
    private IRepository Repository { get; }

    public GetAllCustomersFeature(IRepository repository)
    {
        Repository = repository;
    }

    [HttpGet(CustomerEndpoints.GetAllCustomers)]
    public async Task<GetAllCustomersResponse> GetAll(CancellationToken ct)
    {
        var spec = new Specification();
        var customers = await Repository.ListAsync(spec, ct);
        return new GetAllCustomersResponse(customers);
    }

    public class Specification : Specification<Customer, GetAllCustomersResponse.CustomerDto>
    {
        public Specification()
        {
            Query
                .Select(c => new GetAllCustomersResponse.CustomerDto
                (
                    c.Id,
                    c.CompanyName,
                    c.Location
                ))
                .OrderBy(c => c.CompanyName)
                .AsNoTracking();
        }
    }
}
