﻿using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Modules.Audits;
using Archie.Shared.Modules.Customers;
using Archie.Shared.Modules.Customers.Create;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Modules.Customers;

[ApiController]
public class CreateCustomerModule : IModule
{
    private ICurrentUserService CurrentUser { get; }
    private IRepository Repository { get; }

    public CreateCustomerModule(ICurrentUserService currentUser, IRepository repository)
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

    [NonAction]
    public Audit CustomerCreated(string companyName)
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
