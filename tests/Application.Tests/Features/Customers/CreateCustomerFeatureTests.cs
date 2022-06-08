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
    private static ICurrentUserService AnyCurrentUser => Mock.Of<ICurrentUserService>();
    private static IRepository AnyRepository => Mock.Of<IRepository>();
    private static IValidator<CreateCustomerRequest> AnyValidator => Mock.Of<IValidator<CreateCustomerRequest>>();

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectAuditType()
    {
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidator);
        var result = sut.CustomerCreated(It.IsAny<string>());
        result.AuditType.Should().Be(AuditType.Create);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectEventType()
    {
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidator);
        var result = sut.CustomerCreated(It.IsAny<string>());
        result.EventType.Should().Be(EventType.CustomerCreated);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectDescription()
    {
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidator);
        var result = sut.CustomerCreated("ABC Company");
        result.Description.Should().Be("Customer `ABC Company` was created.");
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCurrentUserId()
    {
        var someUserId = It.IsAny<long>();
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(x => x.Id).Returns(someUserId);

        var sut = new CreateCustomerFeature(mockCurrentUser.Object, AnyRepository, AnyValidator);
        var result = sut.CustomerCreated(It.IsAny<string>());
        result.UserId.Should().Be(someUserId);
    }

    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenValidatorReturnsInvalidValidationResult()
    {
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new InvalidValidationResult());

        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, mockValidator.Object);
        var action = async () => await sut.Create(It.IsAny<CreateCustomerRequest>(), CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectCustomerName()
    {
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var request = new CreateCustomerRequest(It.IsAny<string>(), new Location());
        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, mockValidator.Object);
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
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var location = new Location(city, region, country);
        var request = new CreateCustomerRequest(It.IsAny<string>(), location);
        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, mockValidator.Object);
        await sut.Create(request, CancellationToken.None);

        mockRepository.Verify(x => x.Add(It.Is<Customer>(c => c.Location.Equals(location))));
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectAuditTrail()
    {
        var someRequest = new CreateCustomerRequest(It.IsAny<string>(), new());
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, mockValidator.Object);

        var expectedAudit = sut.CustomerCreated(someRequest.CompanyName);
        await sut.Create(someRequest, CancellationToken.None);

        mockRepository.Verify(_ => _.Add(It.Is<Customer>(c => c.AuditTrail != null && AreEquivalent(c.AuditTrail.First(), expectedAudit))));
    }

    [Fact]
    public async Task Create_ShouldCallSaveChanges()
    {
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, mockValidator.Object);
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
        actual.UserId.Should().Be(expected.UserId);

        return true;
    }
}