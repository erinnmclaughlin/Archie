using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.Audits;

namespace Archie.Application.Features.Customers.Create;

public interface ICreateCustomerAuditService
{
    void AddCreateEventToCustomerAuditTrail(Customer customer);
}

public class CreateCustomerAuditService : ICreateCustomerAuditService
{
    private ICurrentUserService CurrentUser { get; }

    public CreateCustomerAuditService(ICurrentUserService currentUser)
    {
        CurrentUser = currentUser;
    }

    public void AddCreateEventToCustomerAuditTrail(Customer customer)
    {
        customer.AuditTrail ??= new List<Audit>();

        customer.AuditTrail.Add(new Audit
        {
            AuditType = AuditType.Create,
            EventType = EventType.CustomerCreated,
            Description = $"Customer `{customer.CompanyName}` was created.",
            UserId = CurrentUser.Id
        });
    }
}
