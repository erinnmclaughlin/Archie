using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Application.Features.Customers;
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
        var mockAuditService = new Mock<ICreateCustomerAuditService>();
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var request = new CreateCustomerRequest("A Company", new Location());
        var sut = new CreateCustomerFeature(mockAuditService.Object, mockRepository.Object, mockValidator.Object);
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
        var mockAuditService = new Mock<ICreateCustomerAuditService>();
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var location = new Location(city, region, country);
        var request = new CreateCustomerRequest("ABC Company", location);
        var sut = new CreateCustomerFeature(mockAuditService.Object, mockRepository.Object, mockValidator.Object);
        await sut.Create(request, CancellationToken.None);

        mockRepository.Verify(x => x.Add(It.Is<Customer>(c => c.Location.Equals(location))));
    }

    [Fact]
    public async Task Create_ShouldAddToRepository_WithCorrectAuditTrail()
    {
        var someAuditEvent = new Audit();
        var mockAuditService = new Mock<ICreateCustomerAuditService>();
        mockAuditService.Setup(x => x.CustomerCreated(It.IsAny<string>())).Returns(someAuditEvent);
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var sut = new CreateCustomerFeature(mockAuditService.Object, mockRepository.Object, mockValidator.Object);
        await sut.Create(new CreateCustomerRequest(default!, default!), CancellationToken.None);

        mockRepository.Verify(x => x.Add(It.Is<Customer>(c => c.AuditTrail != null && c.AuditTrail.Count == 1 && c.AuditTrail.First().Equals(someAuditEvent))));
    }

    [Fact]
    public async Task Create_ShouldCallSaveChanges()
    {
        var mockAuditService = new Mock<ICreateCustomerAuditService>();
        var mockRepository = new Mock<IRepository>();
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(x => x.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var sut = new CreateCustomerFeature(mockAuditService.Object, mockRepository.Object, mockValidator.Object);
        await sut.Create(new CreateCustomerRequest(default!, default!), CancellationToken.None);

        mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }

    private class InvalidValidationResult : ValidationResult
    {
        public override bool IsValid => false;
    }
}