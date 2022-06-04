using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Shared.Audits;
using Archie.Shared.Customers;
using Archie.Shared.Customers.Create;
using Archie.Shared.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.Customers;

[ApiController]
public class CreateCustomerModule : IModule
{
    private IRepository Repository { get; }

    public CreateCustomerModule(IRepository repository)
    {
        Repository = repository;
    }

    [HttpPost(CustomerEndpoints.CreateCustomer)]
    public async Task<CreateCustomerResponse> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        request.Validate();

        var customer = new Customer
        {
            CompanyName = request.CompanyName,
            City = request.Location.City,
            Region = request.Location.Region,
            Country = request.Location.Country,

            AuditTrail = new List<CustomerAudit>
            {
                new CustomerAudit
                {
                    AuditType = AuditType.Create,
                    Description = $"Customer `{request.CompanyName}` was created.",
                    EventType = CustomerEvent.Created,
                    UserId = 1 // TODO: Don't hard code this ish
                }
            }
        };

        Repository.Add(customer);
        await Repository.SaveChangesAsync(ct);

        return new CreateCustomerResponse
        (
            customer.Id,
            customer.CompanyName,
            new Location
            {
                City = customer.City,
                Region = customer.Region,
                Country = customer.Country
            }
        );
    }
}
