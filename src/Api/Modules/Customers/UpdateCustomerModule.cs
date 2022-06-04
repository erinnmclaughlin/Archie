﻿using Archie.Api.Common;
using Archie.Api.Database.Entities;
using Archie.Api.Exceptions;
using Archie.Shared.Audits;
using Archie.Shared.Customers;
using Archie.Shared.Customers.Update;
using Archie.Shared.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Api.Modules.Customers;

[ApiController]
public class UpdateCustomerModule : IModule
{
    private AuditFactory Audits { get; }
    private IRepository Repository { get; }

    public UpdateCustomerModule(AuditFactory audits, IRepository repository)
    {
        Audits = audits;
        Repository = repository;
    }

    [HttpPatch(CustomerEndpoints.UpdateCustomer)]
    public async Task<UpdateCustomerResponse> Update(long id, UpdateCustomerRequest request, CancellationToken ct)
    {
        request.Validate();

        var customer = await Repository.FindByIdAsync<Customer, long>(id, ct)
            ?? throw new EntityNotFoundException("Customer not found.");

        customer.AuditTrail ??= new List<CustomerAudit>();

        if (!customer.CompanyName.Equals(request.CompanyName))
        {
            customer.AuditTrail.Add(Audits.NameUpdated(customer.CompanyName, request.CompanyName));
            customer.CompanyName = request.CompanyName;
        }

        if (!customer.Location.Equals(request.Location))
        {
            customer.AuditTrail.Add(Audits.LocationUpdated(customer.Location, request.Location));
            customer.Location = request.Location;
        }

        if (customer.AuditTrail.Any())
            await Repository.SaveChangesAsync(ct);

        return new UpdateCustomerResponse(customer.Id, customer.CompanyName, customer.Location);
    }

    public class AuditFactory
    {
        private ICurrentUserService CurrentUser { get; }

        public AuditFactory(ICurrentUserService currentUser)
        {
            CurrentUser = currentUser;
        }

        public virtual CustomerAudit LocationUpdated(Location oldLocation, Location newLocation)
        {
            return new CustomerAudit
            {
                AuditType = AuditType.Update,
                EventType = CustomerEvent.LocationUpdated,
                Description = $"Location was updated from `{oldLocation}` to `{newLocation}`.",
                UserId = CurrentUser.Id
            };
        }

        public virtual CustomerAudit NameUpdated(string oldName, string newName)
        {
            return new CustomerAudit
            {
                AuditType = AuditType.Update,
                EventType = CustomerEvent.NameUpdated,
                Description = $"Company name was updated from `{oldName}` to `{newName}`.",
                UserId = CurrentUser.Id
            };
        }
    }
   

}
