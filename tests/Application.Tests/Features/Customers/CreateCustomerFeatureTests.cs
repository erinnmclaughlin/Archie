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
    private static CreateCustomerRequest AnyRequest => new(It.IsAny<string>(), It.IsAny<Location>());
    private static IValidator<CreateCustomerRequest> AnyValidator => Mock.Of<IValidator<CreateCustomerRequest>>();

    private static IValidator<CreateCustomerRequest> AnyValidatorWithInvalidResult
    {
        get
        {
            var mock = new Mock<IValidator<CreateCustomerRequest>>();
            mock.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new InvalidValidationResult());
            return mock.Object;
        }
    }

    private static IValidator<CreateCustomerRequest> AnyValidatorWithValidResult
    {
        get
        {
            var mock = new Mock<IValidator<CreateCustomerRequest>>();
            mock.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());
            return mock.Object;
        }
    }

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
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidatorWithInvalidResult);
        var action = async () => await sut.Create(It.IsAny<CreateCustomerRequest>(), CancellationToken.None);
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectCustomerName()
    {
        var anyCompanyName = It.IsAny<string>();
        var someRequest = new CreateCustomerRequest(anyCompanyName, It.IsAny<Location>());

        var mockRepository = new Mock<IRepository>();

        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);
        await sut.Create(someRequest, It.IsAny<CancellationToken>());
        mockRepository.Verify(x => x.Add(It.Is<Customer>(c => c.CompanyName == anyCompanyName)));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, null, "USA")]
    [InlineData("Boston", null, "USA")]
    [InlineData(null, "MA", "USA")]
    [InlineData("Boston", "MA", "USA")]
    public async Task Create_ShouldAddToRepository_WithCorrectCustomerLocation(string? city, string? region, string country)
    {
        var someLocation = new Location(city, region, country);
        var someRequest = new CreateCustomerRequest(It.IsAny<string>(), someLocation);

        var mockRepository = new Mock<IRepository>();

        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);
        await sut.Create(someRequest, CancellationToken.None);
        mockRepository.Verify(x => x.Add(It.Is<Customer>(c => c.Location.Equals(someLocation))));
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectAuditTrail()
    {
        var mockRepository = new Mock<IRepository>();

        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);
        var expectedAudit = sut.CustomerCreated(AnyRequest.CompanyName);
        await sut.Create(AnyRequest, CancellationToken.None);
        mockRepository.Verify(_ => _.Add(It.Is<Customer>(c => c.AuditTrail != null && AreEquivalent(c.AuditTrail.First(), expectedAudit))));
    }

    [Fact]
    public async Task Create_ShouldCallSaveChanges()
    {
        var mockRepository = new Mock<IRepository>();

        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);
        await sut.Create(AnyRequest, CancellationToken.None);

        mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Create_ShouldReturnResponse_WithCorrectId()
    {
        var someCustomerId = It.IsAny<long>();

        var mockRepository = new Mock<IRepository>();
        mockRepository.Setup(x => x.Add(It.IsAny<Customer>())).Callback<Customer>(c => c.Id = someCustomerId);

        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);
        var response = await sut.Create(AnyRequest, It.IsAny<CancellationToken>());
        response.Id.Should().Be(someCustomerId);
    }

    private static bool AreEquivalent(Audit actual, Audit expected)
    {
        actual.AuditType.Should().Be(expected.AuditType);
        actual.Description.Should().Be(expected.Description);
        actual.EventType.Should().Be(expected.EventType);
        actual.UserId.Should().Be(expected.UserId);

        return true;
    }

    private class InvalidValidationResult : ValidationResult
    {
        public override bool IsValid => false;
    }
}