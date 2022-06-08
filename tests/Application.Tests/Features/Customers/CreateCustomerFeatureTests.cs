using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Application.Features.Customers;
using Archie.Shared.Features.Audits;
using Archie.Shared.Features.Customers.Create;
using Archie.Shared.ValueObjects;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Archie.Application.Tests.Features.Customers;

public class CreateCustomerFeatureTests
{
    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectAuditType()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id);

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, default!, default!);
        var result = sut.CustomerCreated("Some Company Name");
        result.AuditType.Should().Be(AuditType.Create);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectEventType()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id);

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, default!, default!);
        var result = sut.CustomerCreated("Acme Corp");
        result.EventType.Should().Be(EventType.CustomerCreated);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectDescription()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id);

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, default!, default!);
        var result = sut.CustomerCreated("ABC Company");
        result.Description.Should().Be("Customer `ABC Company` was created.");
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCurrentUserId()
    {
        var someUserId = 123;

        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id).Returns(someUserId);

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, default!, default!);
        var result = sut.CustomerCreated("A Company");
        result.UserId.Should().Be(someUserId);
    }

    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenValidatorReturnsInvalidValidationResult()
    {
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new InvalidValidationResult());

        var sut = new CreateCustomerFeature(default!, default!, mockValidator.Object);
        var action = async () => await sut.Create(default!, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectCustomerName()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var request = new CreateCustomerRequest("A Company", new Location());
        var sut = new CreateCustomerFeature(mockCurrentUser.Object, mockRepository.Object, mockValidator.Object);
        await sut.Create(request, CancellationToken.None);

        mockRepository.Verify(x => x.Add(It.Is<Customer>(c => c.CompanyName == request.CompanyName)));
    }

    [Theory]
    [InlineData(null, null, "USA")]
    [InlineData("Boston", null, "USA")]
    [InlineData(null, "MA", "USA")]
    [InlineData("Boston", "MA", "USA")]
    public async Task Create_ShouldAddToRepository_WithCorrectCustomerLocation(string? city, string? region, string country)
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var location = new Location(city, region, country);
        var request = new CreateCustomerRequest("ABC Company", location);
        var sut = new CreateCustomerFeature(mockCurrentUser.Object, mockRepository.Object, mockValidator.Object);
        await sut.Create(request, CancellationToken.None);

        mockRepository.Verify(x => x.Add(It.Is<Customer>(c => c.Location.Equals(location))));
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectAuditTrail()
    {
        var someRequest = new CreateCustomerRequest("Some Name", new());
        var mockCurrentUser = new Mock<ICurrentUserService>();
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, mockRepository.Object, mockValidator.Object);

        var expectedAudit = sut.CustomerCreated(someRequest.CompanyName);
        await sut.Create(someRequest, CancellationToken.None);

        mockRepository.Verify(_ => _.Add(It.Is<Customer>(c => c.AuditTrail != null && AreEquivalent(c.AuditTrail.First(), expectedAudit))));
    }

    [Fact]
    public async Task Create_ShouldCallSaveChanges()
    {
        var mockCurrentUser = new Mock<ICurrentUserService>();
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, mockRepository.Object, mockValidator.Object);
        await sut.Create(new CreateCustomerRequest(default!, default!), CancellationToken.None);

        mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    private class InvalidValidationResult : ValidationResult
    {
        public override bool IsValid => false;
    }

    private static bool AreEquivalent(Audit actual, Audit expected)
    {
        actual.AuditType.Should().Be(expected.AuditType);
        actual.Description.Should().Be(expected.Description);
        actual.EventType.Should().Be(expected.EventType);

        return true;
    }
}