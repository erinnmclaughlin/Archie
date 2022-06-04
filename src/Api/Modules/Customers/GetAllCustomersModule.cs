using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Shared.Customers;
using Archie.Shared.Customers.GetAll;
using Archie.Shared.ValueObjects;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.Customers;

[ApiController]
public class GetAllCustomersModule : IModule
{
    private IRepository Repository { get; }

    public GetAllCustomersModule(IRepository repository)
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
                    new Location(c.City, c.Region, c.Country)
                ))
                .OrderBy(c => c.CompanyName)
                .AsNoTracking();
        }
    }
}
