using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Api.Exceptions;
using Archie.Shared.Customers;
using Archie.Shared.Customers.GetDetails;
using Ardalis.Specification;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.Customers;

[ApiController]
public class GetCustomerDetailsModule : IModule
{
    private IRepository Repository { get; }

    public GetCustomerDetailsModule(IRepository repository)
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
                ))
                .Where(c => c.Id == id)
                .AsNoTracking();
        }
    }
}
