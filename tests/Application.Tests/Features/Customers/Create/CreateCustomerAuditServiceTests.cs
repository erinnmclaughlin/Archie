using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Application.Features.Customers.Create;
using Archie.Shared.Features.Audits;
using Archie.Shared.ValueObjects;
using FluentAssertions;
using Moq;

namespace Archie.Application.Tests.Features.Customers.Create;

public class CreateCustomerAuditServiceTests
{
    [Fact]
    public void AddCreateEventToCustomerAuditTrail_ShouldAddCorrectInfo()
    {
        // Arrange
        var anyCustomer = new Customer
        {
            CompanyName = It.IsAny<string>(),
            Location = It.IsAny<Location>()
        };

        var anyCurrentUser = Mock.Of<ICurrentUserService>();
        var sut = new CreateCustomerAuditService(anyCurrentUser);

        // Act
        sut.AddCreateEventToCustomerAuditTrail(anyCustomer);

        // Assert
        var audit = anyCustomer.AuditTrail.Should().ContainSingle().Which;
        audit.AuditType.Should().Be(AuditType.Create);
        audit.EventType.Should().Be(EventType.CustomerCreated);
        audit.Description.Should().Be($"Customer `{anyCustomer.CompanyName}` was created.");
        audit.UserId.Should().Be(anyCurrentUser.Id);
    }
}
