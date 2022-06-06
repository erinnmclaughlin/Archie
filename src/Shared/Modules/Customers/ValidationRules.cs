using FluentValidation;

namespace Archie.Shared.Modules.Customers;

public static class ValidationRules
{
    public static IRuleBuilderOptions<T, string?> IsValidCompanyName<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.");
    }

    public static IRuleBuilderOptions<T, string?> IsValidCity<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(100).WithMessage("City cannot exceed 100 characters.");
    }

    public static IRuleBuilderOptions<T, string?> IsValidRegion<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(100).WithMessage("Region cannot exceed 100 characters.");
    }

    public static IRuleBuilderOptions<T, string?> IsValidCountry<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.NotEmpty().WithMessage("Country is required.")
            .MaximumLength(200).WithMessage("Country cannot exceed 200 characters.");
    }
}
