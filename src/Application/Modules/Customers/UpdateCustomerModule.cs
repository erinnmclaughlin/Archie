using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Application.Exceptions;
using Archie.Shared.Modules.Audits;
using Archie.Shared.Modules.Customers;
using Archie.Shared.Modules.Customers.Update;
using Archie.Shared.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Modules.Customers;

[ApiController]
public class UpdateCustomerModule : IModule
{
    private ICurrentUserService CurrentUser { get; }
    private IRepository Repository { get; }

    public UpdateCustomerModule(ICurrentUserService currentUser, IRepository repository)
    {
        CurrentUser = currentUser;
        Repository = repository;
    }

    [HttpPatch(CustomerEndpoints.UpdateCustomer)]
    public async Task<UpdateCustomerResponse> Update(long id, UpdateCustomerRequest request, CancellationToken ct)
    {
        request.Validate();

        var customer = await Repository.FindByIdAsync<Customer, long>(id, ct)
            ?? throw new EntityNotFoundException("Customer not found.");

        customer.AuditTrail ??= new List<Audit>();

        if (!customer.CompanyName.Equals(request.CompanyName))
        {
            customer.AuditTrail.Add(NameUpdated(customer.CompanyName, request.CompanyName));
            customer.CompanyName = request.CompanyName;
        }

        if (!customer.Location.Equals(request.Location))
        {
            customer.AuditTrail.Add(LocationUpdated(customer.Location, request.Location));
            customer.Location = request.Location;
        }

        if (customer.AuditTrail.Any())
            await Repository.SaveChangesAsync(ct);

        return new UpdateCustomerResponse(customer.Id, customer.CompanyName, customer.Location);
    }

    [NonAction]
    public Audit LocationUpdated(Location oldLocation, Location newLocation)
    {
        return new Audit
        {
            AuditType = AuditType.Update,
            EventType = EventType.CustomerLocationUpdated,
            Description = $"Location was updated from `{oldLocation}` to `{newLocation}`.",
            UserId = CurrentUser.Id
        };
    }

    [NonAction]
    public Audit NameUpdated(string oldName, string newName)
    {
        return new Audit
        {
            AuditType = AuditType.Update,
            EventType = EventType.CustomerNameUpdated,
            Description = $"Company name was updated from `{oldName}` to `{newName}`.",
            UserId = CurrentUser.Id
        };
    }
}
