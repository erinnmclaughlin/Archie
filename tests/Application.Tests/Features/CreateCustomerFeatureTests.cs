using Archie.Application.Features.Customers;
using Archie.Shared.Features.Customers.Create;
using Archie.Shared.ValueObjects;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
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
        var sut = new CreateCustomerFeature(default!, default!, default!, validator);

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
        var sut = new CreateCustomerFeature(default!, default!, default!, mockValidator.Object);

        // Assert
        await sut.Invoking(_ => _.Create(someInvalidRequest, It.IsAny<CancellationToken>()))
            .Should().ThrowAsync<ValidationException>();
    }
}
