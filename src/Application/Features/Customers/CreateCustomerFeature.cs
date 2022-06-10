using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.Create;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.Customers;

[ApiController]
public class CreateCustomerFeature : IFeature
{
    private ICurrentUserService CurrentUser { get; }
    private IRepository Repository { get; }
    private IValidator<CreateCustomerRequest> Validator { get; }

    public CreateCustomerFeature(ICurrentUserService currentUser, IRepository repository, IValidator<CreateCustomerRequest> validator)
    {
        CurrentUser = currentUser;
        Repository = repository;
        Validator = validator;
    }

    [HttpPost(CustomerEndpoints.CreateCustomer)]
    public async Task<CreateCustomerResponse> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        ValidateRequestOrThrow(request);

        var customer = CreateCustomerWithAuditTrail(request);

        Repository.Add(customer);
        await Repository.SaveChangesAsync(ct);

        return new CreateCustomerResponse(customer.Id);
    }

    private Customer CreateCustomerWithAuditTrail(CreateCustomerRequest request)
    {
        return new Customer
        {
            CompanyName = request.CompanyName,
            Location = request.Location,
            AuditTrail = new List<Audit> 
            { 
                GetCustomerCreatedEvent(request.CompanyName) 
            }
        };
    }

    private void ValidateRequestOrThrow(CreateCustomerRequest request)
    {
        var validationResult = Validator.Validate(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);
    }

    private Audit GetCustomerCreatedEvent(string companyName)
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
