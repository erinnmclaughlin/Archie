using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.Create;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.Customers;

public interface ICreateCustomerAuditService
{
    Audit CustomerCreated(string companyName);
}

[ApiController]
public class CreateCustomerFeature
{
    private ICreateCustomerAuditService Audits { get; }
    private IRepository Repository { get; }
    private IValidator<CreateCustomerRequest> Validator { get; }

    public CreateCustomerFeature(ICreateCustomerAuditService audits, IRepository repository, IValidator<CreateCustomerRequest> validator)
    {
        Audits = audits;
        Repository = repository;
        Validator = validator;
    }

    [HttpPost(CustomerEndpoints.CreateCustomer)]
    public async Task<CreateCustomerResponse> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        var validationResult = Validator.Validate(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var customer = new Customer
        {
            CompanyName = request.CompanyName,
            Location = request.Location,

            AuditTrail = new List<Audit> { Audits.CustomerCreated(request.CompanyName) }
        };

        Repository.Add(customer);
        await Repository.SaveChangesAsync(ct);

        return new CreateCustomerResponse(customer.Id);
    }

    public class AuditService : ICreateCustomerAuditService
    {
        private ICurrentUserService CurrentUser { get; }

        public AuditService(ICurrentUserService currentUser)
        {
            CurrentUser = currentUser;
        }

        public virtual Audit CustomerCreated(string companyName)
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
}
