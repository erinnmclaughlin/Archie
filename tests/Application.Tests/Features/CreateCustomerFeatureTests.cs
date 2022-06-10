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
    /// <summary>
    /// Pros I see with a test like this:
    /// 1. It's simple & easy to understand
    /// 
    /// Cons I see with a test like this:
    /// 1. We're relying on `CreateCustomerRequest.Validator` to be the implementation of IValidator
    /// 2. We're relying on our chosen "invalid" request example to be representative of ALL invalid requests
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenRequestIsInvalid_Classic()
    {
        // Arrange
        var validator = new CreateCustomerRequest.Validator();
        var someInvalidRequest = new CreateCustomerRequest("A Company without a Country", new Location());

        // Act
        var sut = new CreateCustomerFeature(default!, default!, validator);

        // Assert
        await sut.Invoking(_ => _.Create(someInvalidRequest, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenRequestIsInvalid_London()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>()))
            .Returns(new ValidationResult(new List<ValidationFailure> { new("Some Property", "Some Error") }));

        var someInvalidRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());

        // Act
        var sut = new CreateCustomerFeature(default!, default!, mockValidator.Object);

        // Assert
        await sut.Invoking(_ => _.Create(someInvalidRequest, It.IsAny<CancellationToken>()))
            .Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldAddCorrectCustomerToRepository()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(_ => _.Id).Returns(It.IsAny<long>());

        var mockRepository = new Mock<IRepository>();

        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var someRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, mockRepository.Object, mockValidator.Object);
        await sut.Create(someRequest, It.IsAny<CancellationToken>());

        mockRepository.Verify(_ => _.Add(It.Is<Customer>(c => IsExpected(c, someRequest, mockCurrentUser.Object.Id))), Times.Once);
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
        var sut = new CreateCustomerFeature(mockCurrentUser.Object, repository, mockValidator.Object);
        var response = await sut.Create(someValidRequest, It.IsAny<CancellationToken>());

        // Assert
        var customer = context.Customers.Include(c => c.AuditTrail).Single();
        IsExpected(customer, someValidRequest, mockCurrentUser.Object.Id).Should().BeTrue();
    }

    private static bool IsExpected(Customer customer, CreateCustomerRequest request, long currentUserId)
    {
        customer.CompanyName.Should().Be(request.CompanyName);
        customer.Location.Should().BeEquivalentTo(request.Location);

        customer.AuditTrail.Should().NotBeNull();
        var audit = customer.AuditTrail.Should().ContainSingle().Which;
        audit.AuditType.Should().Be(AuditType.Create);
        audit.Description.Should().Be($"Customer `{request.CompanyName}` was created.");
        audit.EventType.Should().Be(EventType.CustomerCreated);
        audit.UserId.Should().Be(currentUserId);

        return true;
    }
}
