using Archie.Application.Database.Entities;
using Archie.Shared.Features.Customers.Create;
using AutoMapper;

namespace Archie.Application.Features.Customers.Create;

public class CreateCustomerMappingProfile : Profile
{
    public CreateCustomerMappingProfile()
    {
        CreateMap<CreateCustomerRequest, Customer>();
        CreateMap<Customer, CreateCustomerResponse>();
    }
}
