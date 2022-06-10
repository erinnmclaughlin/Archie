using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Shared.Features.Customers;
using Archie.Shared.Features.Customers.Create;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Archie.Application.Features.Customers.Create;

[ApiController]
public class CreateCustomerFeature
{
    private ICreateCustomerAuditService AuditService { get; }
    private IMapper Mapper { get; }
    private IRepository Repository { get; }
    private IValidator<CreateCustomerRequest> Validator { get; }

    public CreateCustomerFeature(ICreateCustomerAuditService auditService, IMapper mapper, IRepository repository, IValidator<CreateCustomerRequest> validator)
    {
        AuditService = auditService;
        Mapper = mapper;
        Repository = repository;
        Validator = validator;
    }

    [HttpPost(CustomerEndpoints.CreateCustomer)]
    public async Task<CreateCustomerResponse> Create(CreateCustomerRequest request, CancellationToken ct)
    {
        Validator.ValidateAndThrow(request);

        var customer = Mapper.Map<Customer>(request); 
        AuditService.AddCreateEventToCustomerAuditTrail(customer);

        Repository.Add(customer);
        await Repository.SaveChangesAsync(ct);

        return Mapper.Map<CreateCustomerResponse>(customer);
    }
}
