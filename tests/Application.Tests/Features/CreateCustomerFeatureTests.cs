using Archie.Application.Common;
using Archie.Application.Database;
using Archie.Application.Database.Entities;
using Archie.Application.Features.Customers;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.Customers.Create;
using Archie.Shared.ValueObjects;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Archie.Application.Tests.Features;

public class CreateCustomerFeatureTests
{
    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>()))
            .Returns(new ValidationResult(new List<ValidationFailure> { new("Some Property", "Some Error") }));

        var someInvalidRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());

        // Act & Assert
        await new CreateCustomerFeature(default!, default!, mockValidator.Object)
            .Invoking(_ => _.Create(someInvalidRequest, It.IsAny<CancellationToken>()))
            .Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldAddCorrectCustomerToRepository()
    {
        // Arrange
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(_ => _.Id).Returns(It.IsAny<long>());
        var currentUser = mockCurrentUser.Object;

        Customer? customer = null;
        var mockRepository = new Mock<IRepository>();
        mockRepository.Setup(_ => _.Add(It.IsAny<Customer>())).Callback<Customer>(c => customer = c);

        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var someRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());

        // Act
        await new CreateCustomerFeature(currentUser, mockRepository.Object, mockValidator.Object)
            .Create(someRequest, It.IsAny<CancellationToken>());

        // Assert
        customer.Should().NotBeNull();
        customer!.CompanyName.Should().Be(someRequest.CompanyName);
        customer.Location.Should().BeEquivalentTo(someRequest.Location);
        customer.AuditTrail.Should().NotBeNull();
        var audit = customer.AuditTrail.Should().ContainSingle().Which;
        audit.AuditType.Should().Be(AuditType.Create);
        audit.Description.Should().Be($"Customer `{someRequest.CompanyName}` was created.");
        audit.EventType.Should().Be(EventType.CustomerCreated);
        audit.UserId.Should().Be(currentUser.Id);
    }

    [Fact]
    public async Task Create_ShouldAddCustomerAndAuditEventToDatabase()
    {
        // Arrange
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(_ => _.Id).Returns(It.IsAny<long>());

        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        using var context = new InMemoryArchieContext();
        var repository = new ArchieRepository(context);
        var someValidRequest = new CreateCustomerRequest("ABC Company", new Location("Boston", "MA", "USA"));

        // Act
        var response = await new CreateCustomerFeature(mockCurrentUser.Object, repository, mockValidator.Object)
            .Create(someValidRequest, It.IsAny<CancellationToken>());

        // Assert
        var customer = context.Customers.Include(c => c.AuditTrail).SingleOrDefault();
        customer.Should().NotBeNull();
        customer?.AuditTrail.Should().ContainSingle();
    }
}
