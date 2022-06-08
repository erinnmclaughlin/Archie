using Archie.Application.Common;
using Archie.Application.Features.Customers;
using Archie.Shared.Features.Audits;
using FluentAssertions;
using Moq;

namespace Archie.Application.Tests.Features.Customers.Create;

public class CreateCustomerFeatureAuditServiceTests
{
    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectAuditType()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id);

        var sut = new CreateCustomerFeature.AuditService(mockCurrentUser.Object);
        var result = sut.CustomerCreated("Some Company Name");
        result.AuditType.Should().Be(AuditType.Create);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectEventType()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id);

        var sut = new CreateCustomerFeature.AuditService(mockCurrentUser.Object);
        var result = sut.CustomerCreated("Acme Corp");
        result.EventType.Should().Be(EventType.CustomerCreated);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectDescription()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id);

        var sut = new CreateCustomerFeature.AuditService(mockCurrentUser.Object);
        var result = sut.CustomerCreated("ABC Company");
        result.Description.Should().Be("Customer `ABC Company` was created.");
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCurrentUserId()
    {
        var someUserId = 123;

        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id).Returns(someUserId);

        var sut = new CreateCustomerFeature.AuditService(mockCurrentUser.Object);
        var result = sut.CustomerCreated("A Company");
        result.UserId.Should().Be(someUserId);
    }
}
