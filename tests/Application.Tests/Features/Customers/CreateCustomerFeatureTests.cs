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
            mock.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new InvalidValidationResult());
            return mock.Object;
        }
    }

    private static IValidator<CreateCustomerRequest> AnyValidatorWithValidResult
    {
        get
        {
            var mock = new Mock<IValidator<CreateCustomerRequest>>();
            mock.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());
            return mock.Object;
        }
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectAuditType()
    {
        // Given any current user, any repository, any validator...
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidator);

        // ...and any name...
        var result = sut.CustomerCreated(It.IsAny<string>());

        // ...the resulting Audit should have AuditType set to AuditType.Create
        result.AuditType.Should().Be(AuditType.Create);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectEventType()
    {
        // Given any current user, any repository, any validator...
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidator);

        // ...and any name...
        var result = sut.CustomerCreated(It.IsAny<string>());

        // ...the resulting Audit should have EventType set to EventType.CustomerCreated
        result.EventType.Should().Be(EventType.CustomerCreated);
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCorrectDescription()
    {
        // Given some name...
        var someName = It.IsAny<string>();

        // and any current user, any repository, and any validator...
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidator);
        var result = sut.CustomerCreated(someName);

        // ...the resulting Audit should have Description set to:
        result.Description.Should().Be($"Customer `{someName}` was created.");
    }

    [Fact]
    public void CustomerCreated_ShouldReturnAuditWithCurrentUserId()
    {
        // Given a current user service that returns some id...
        var mockCurrentUser = new Mock<ICurrentUserService>();
        mockCurrentUser.SetupGet(_ => _.Id).Returns(It.IsAny<long>());

        // ...and any repository, any validator...
        var sut = new CreateCustomerFeature(mockCurrentUser.Object, AnyRepository, AnyValidator);

        // ...and any name...
        var result = sut.CustomerCreated(It.IsAny<string>());

        // ...the resulting Audit should have UserId that matches whatever was returned from the current user service
        result.UserId.Should().Be(mockCurrentUser.Object.Id);
    }

    [Fact]
    public async Task Create_ShouldThrowValidationException_WhenValidatorReturnsInvalidValidationResult()
    {
        // Given any current user, any repository, any validator that returns an INVALID response...
        var sut = new CreateCustomerFeature(AnyCurrentUser, AnyRepository, AnyValidatorWithInvalidResult);

        // ...and any CancellationToken...
        var action = async () => await sut.Create(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>());

        // ...a ValidationException should be thrown
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectCustomerName()
    {
        // Given a request with any company name...
        var someRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());

        var mockRepository = new Mock<IRepository>();

        // ...and any current user, any repository, any validator that returns a VALID response...
        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);

        // ...and any CancellationToken...
        await sut.Create(someRequest, It.IsAny<CancellationToken>());

        // ...a Customer object with a company name matching the request company name should be passed to Repository.Add()
        mockRepository.Verify(_ => _.Add(It.Is<Customer>(c => c.CompanyName == someRequest.CompanyName)));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(null, null, "USA")]
    [InlineData("Boston", null, "USA")]
    [InlineData(null, "MA", "USA")]
    [InlineData("Boston", "MA", "USA")]
    public async Task Create_ShouldAddToRepository_WithCorrectCustomerLocation(string? city, string? region, string country)
    {
        // Given a request with any location...
        var someRequest = new CreateCustomerRequest(It.IsAny<string>(), new Location(city, region, country));

        var mockRepository = new Mock<IRepository>();

        // ...any any current user, any repository, any validator that returns a VALID response...
        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);

        // ...and any CancellationToken...
        await sut.Create(someRequest, It.IsAny<CancellationToken>());

        // ...a Customer object with a location matching the request location should be passed to Repository.Add()
        mockRepository.Verify(_ => _.Add(It.Is<Customer>(c => c.Location.Equals(someRequest.Location))));
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectAuditTrail()
    {
        var mockRepository = new Mock<IRepository>();

        // Given any current user, any repository, any validator that returns a VALID reponse...
        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);
        
        // ...and any CancellationToken...
        await sut.Create(AnyRequest, It.IsAny<CancellationToken>()); 
        
        // ...a Customer object with an audit trail containing a single item, where that item meets the criteria specified in "AreEquivalent" (below), should be passed to Repository.Add()
        mockRepository.Verify(_ => _.Add(It.Is<Customer>(c => c.AuditTrail != null && AreEquivalent(c.AuditTrail.First(), sut.CustomerCreated(AnyRequest.CompanyName)))));
    }

    [Fact]
    public async Task Create_ShouldCallSaveChanges()
    {
        var mockRepository = new Mock<IRepository>();

        // Given any current user, any repository, any validator that returns a VALID response...
        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);

        // ...and any CancellationToken...
        var someCancellationToken = It.IsAny<CancellationToken>();
        await sut.Create(AnyRequest, someCancellationToken);

        // ...that CancellationToken is passed to Repository.SaveChangesAsync()
        mockRepository.Verify(_ => _.SaveChangesAsync(It.Is<CancellationToken>(ct => ct == someCancellationToken)));
    }

    [Fact]
    public async Task Create_ShouldReturnResponse_WithCorrectId()
    {
        var someCustomerId = It.IsAny<long>();
        
        // Given some repository that sets the given Customer.Id to some value...
        var mockRepository = new Mock<IRepository>();
        mockRepository.Setup(_ => _.Add(It.IsAny<Customer>())).Callback<Customer>(c => c.Id = someCustomerId);

        // ...and any current user, any validator that returns a VALID response...
        var sut = new CreateCustomerFeature(AnyCurrentUser, mockRepository.Object, AnyValidatorWithValidResult);

        // ...and any CancellationToken...
        var response = await sut.Create(AnyRequest, It.IsAny<CancellationToken>());

        // ...a CreateCustomerResponse object should be returned with an Id that matches value assigned by the repository
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