using Archie.Application.Database;
using Archie.Application.Features.Customers;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.Customers.Create;
using Archie.Shared.ValueObjects;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Archie.Application.Tests.Features.Customers;

/// <summary>
/// PROS OF THIS APPROACH:
/// 1. Implementation agnostic
///    a. Scales well with change
///    b. Supports TDD
/// 2. Simpler tests / easier to understand
/// 
/// CONS OF THIS APPROACH:
/// 1. Does not FULLY cover all possible cases
///    a. e.g., attempting to prove "for all" by proving "for one"
/// 2. Requires in memory db (slower tests)
/// </summary>

public class CreateCustomerFeatureTests
{
    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        using var context = new TestArchieContext();
        var currentUser = new TestCurrentUser();
        var repository = new ArchieRepository(context);
        var someInvalidRequest = new CreateCustomerRequest("A Company without a Country", new Location());

        // Act
        var sut = new CreateCustomerFeature(currentUser, repository);

        // Assert
        await sut.Invoking(_ => _.Create(someInvalidRequest, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldReturnCorrectResponse()
    {
        // Arrange
        using var context = new TestArchieContext();
        var currentUser = new TestCurrentUser();
        var repository = new ArchieRepository(context);
        var someValidRequest = new CreateCustomerRequest("ABC Company", new Location("Charlottesville", null, "USA"));

        // Act
        var sut = new CreateCustomerFeature(currentUser, repository);
        var response = await sut.Create(someValidRequest, CancellationToken.None);
        var customerInDb = context.Customers.Single();

        response.Should().BeOfType<CreateCustomerResponse>().Which.Id.Should().Be(customerInDb.Id);
    }

    [Fact]
    public async Task Create_ShouldAddCustomerWithCorrectInfoToDatabase()
    {
        // Arrange
        using var context = new TestArchieContext();
        var currentUser = new TestCurrentUser();
        var repository = new ArchieRepository(context);
        var someValidRequest = new CreateCustomerRequest("ABC Company", new Location("Charlottesville", null, "USA"));

        // Act
        var sut = new CreateCustomerFeature(currentUser, repository);
        var response = await sut.Create(someValidRequest, CancellationToken.None);
        var customerInDb = context.Customers.Single();

        // Assert
        customerInDb.CompanyName.Should().Be(someValidRequest.CompanyName);
        customerInDb.Location.Should().BeEquivalentTo(someValidRequest.Location);
    }

    [Fact]
    public async Task Create_ShouldAddCorrectAuditWithCorrectInfoToDatabase()
    {
        // Arrange
        using var context = new TestArchieContext();
        var currentUser = new TestCurrentUser();
        var repository = new ArchieRepository(context);
        var someValidRequest = new CreateCustomerRequest("ABC Company", new Location("Charlottesville", null, "USA"));

        // Act
        var sut = new CreateCustomerFeature(currentUser, repository);
        var response = await sut.Create(someValidRequest, CancellationToken.None);
        var auditEvents = context.Customers.Include(c => c.AuditTrail).SelectMany(c => c.AuditTrail!).ToList();

        // Assert
        var audit = auditEvents.Should().ContainSingle().Which;
        audit.AuditType.Should().Be(AuditType.Create);
        audit.EventType.Should().Be(EventType.CustomerCreated);
        audit.Description.Should().Be("Customer `ABC Company` was created.");
        audit.UserId.Should().Be(currentUser.Id);
    }
}
