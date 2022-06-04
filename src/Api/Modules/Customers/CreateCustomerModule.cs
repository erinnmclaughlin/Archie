using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Shared.Audits;
using Archie.Shared.Customers;
using Archie.Shared.Customers.Create;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.Customers;

[ApiController]
public class CreateCustomerModule : IModule
{
    private AuditFactory Audits { get; }
    private IRepository Repository { get; }

    public CreateCustomerModule(AuditFactory audits, IRepository repository)
    {
        Audits = audits;
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

            AuditTrail = new List<CustomerAudit> { Audits.CustomerCreated(request.CompanyName) }
        };

        Repository.Add(customer);
        await Repository.SaveChangesAsync(ct);

        return new CreateCustomerResponse
        (
            customer.Id,
            customer.CompanyName,
            customer.Location
        );
    }

    public class AuditFactory
    {
        private ICurrentUserService CurrentUser { get; }

        public AuditFactory(ICurrentUserService currentUser)
        {
            CurrentUser = currentUser;
        }

        public virtual CustomerAudit CustomerCreated(string companyName)
        {
            return new CustomerAudit
            {
                AuditType = AuditType.Create,
                EventType = CustomerEvent.Created,
                Description = $"Customer `{companyName}` was created.",
                UserId = CurrentUser.Id
            };
        }
    }
}
