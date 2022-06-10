using Archie.Application.Database.Entities;
using Archie.Application.Features.Customers.Create;
using Archie.Shared.Features.Customers.Create;
using Archie.Shared.ValueObjects;
using FluentAssertions;
using Moq;

namespace Archie.Application.Tests.Features.Customers.Create;

public class CreateCustomerMappingProfileTests
{
    [Fact]
    public void CreateCustomerMappingProfile_MapsFromCreateCustomerRequestToCustomer()
    {
        // Arrange
        var anyRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());

        // Act
        var mapper = new CreateCustomerMappingProfile().CreateMapper();
        var customer = mapper.Map<Customer>(anyRequest);

        // Assert
        customer.CompanyName.Should().Be(anyRequest.CompanyName);
        customer.Location.Should().BeEquivalentTo(anyRequest.Location);
    }
}
