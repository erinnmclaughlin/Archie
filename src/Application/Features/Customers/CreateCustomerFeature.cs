using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.Create;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.Customers;

[ApiController]
public class CreateCustomerFeature : IFeature
{
    private ICurrentUserService CurrentUser { get; }
    private IMapper Mapper { get; }
    private IRepository Repository { get; }
    private IValidator<CreateCustomerRequest> Validator { get; }

    public CreateCustomerFeature(ICurrentUserService currentUser, IMapper mapper, IRepository repository, IValidator<CreateCustomerRequest> validator)
    {
        CurrentUser = currentUser;
        Mapper = mapper;
        Repository = repository;
        Validator = validator;
    }

    [HttpPost(CustomerEndpoints.CreateCustomer)]
    public async Task<CreateCustomerResponse> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        var validationResult = Validator.Validate(request);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var customer = Mapper.Map<Customer>(request);
        customer.AuditTrail = new List<Audit> { CustomerCreated(request.CompanyName) };

        Repository.Add(customer);
        await Repository.SaveChangesAsync(ct);

        return Mapper.Map<CreateCustomerResponse>(customer);
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

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateCustomerRequest, Customer>();
            CreateMap<Customer, CreateCustomerResponse>();
        }
    }
}
