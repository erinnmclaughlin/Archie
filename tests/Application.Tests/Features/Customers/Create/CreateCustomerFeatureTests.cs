using Archie.Application.Common;
using Archie.Application.Database.Entities;
using Archie.Application.Features.Customers.Create;
using Archie.Shared.Features.Customers.Create;
using Archie.Shared.ValueObjects;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Archie.Application.Tests.Features.Customers.Create;

public class CreateCustomerFeatureTests
{
    [Fact]
    public async Task Create_ShouldThrowValidationExceptionWhenValidatorReturnsInvalidResult()
    {
        // Arrange
        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>()))
            .Returns(new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Some Property", "Some error message")
            }));
        var anyRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());
        var sut = new CreateCustomerFeature
        (
            Mock.Of<ICreateCustomerAuditService>(),
            Mock.Of<IMapper>(),
            Mock.Of<IRepository>(),
            mockValidator.Object
        );

        // Act & Assert
        await sut.Invoking(_ => _.Create(anyRequest, It.IsAny<CancellationToken>()))
            .Should()
            .ThrowAsync<ValidationException>();

        mockValidator.Verify(x => x.Validate(anyRequest), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldPassMappedCustomerToAuditServiceAndRepository()
    {
        // Arrange
        var mockAuditService = new Mock<ICreateCustomerAuditService>();

        var someMappedCustomer = Mock.Of<Customer>();
        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(_ => _.Map<Customer>(It.IsAny<CreateCustomerRequest>())).Returns(someMappedCustomer);

        var mockRepository = new Mock<IRepository>();

        var mockValidator = new Mock<IValidator<CreateCustomerRequest>>();
        mockValidator.Setup(_ => _.Validate(It.IsAny<CreateCustomerRequest>())).Returns(new ValidationResult());

        var anyRequest = new CreateCustomerRequest(It.IsAny<string>(), It.IsAny<Location>());
        var sut = new CreateCustomerFeature(mockAuditService.Object, mockMapper.Object, mockRepository.Object, mockValidator.Object);

        // Act
        await sut.Create(anyRequest, It.IsAny<CancellationToken>());

        // Assert
        mockValidator.Verify(x => x.Validate(anyRequest), Times.Once);
        mockMapper.Verify(x => x.Map<Customer>(anyRequest), Times.Once);
        mockAuditService.Verify(x => x.AddCreateEventToCustomerAuditTrail(someMappedCustomer), Times.Once);
        mockRepository.Verify(x => x.Add(someMappedCustomer), Times.Once);
        mockRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
