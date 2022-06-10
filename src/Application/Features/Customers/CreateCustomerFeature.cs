using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.Create;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.Customers;

[ApiController]
public class CreateCustomerFeature : IFeature
{
    private ICurrentUserService CurrentUser { get; }
    private IRepository Repository { get; }

    public CreateCustomerFeature(ICurrentUserService currentUser, IRepository repository)
    {
        CurrentUser = currentUser;
        Repository = repository;
    }

    [HttpPost(CustomerEndpoints.CreateCustomer)]
    public async Task<CreateCustomerResponse> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        request.Validate();

        var customer = new Customer
        {
            CompanyName = request.CompanyName,
            Location = request.Location,

            AuditTrail = new List<Audit> { CustomerCreated(request.CompanyName) }
        };

        Repository.Add(customer);
        await Repository.SaveChangesAsync(ct);

        return new CreateCustomerResponse(customer.Id);
    }

    private Audit CustomerCreated(string companyName)
    {
        return new Audit
        {
            AuditType = AuditType.Create,
            EventType = EventType.CustomerCreated,
            Description = $"Customer `{companyName}` was created.",
            UserId = CurrentUser.Id
        };
    }
}
