using Archie.Shared.ValueObjects;
using FluentValidation;

namespace Archie.Shared.Features.Customers.Create;

public class CreateCustomerRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public Location Location { get; set; }

    public CreateCustomerRequest(string companyName, Location location)
    {
        CompanyName = companyName;
        Location = location;
    }

    public void Validate() => new Validator().ValidateAndThrow(this);

    public class Validator : AbstractValidator<CreateCustomerRequest>
    {
        public Validator()
        {
            RuleFor(x => x.CompanyName).IsValidCompanyName();
            RuleFor(x => x.Location.City).IsValidCity();
            RuleFor(x => x.Location.Region).IsValidRegion();
            RuleFor(x => x.Location.Country).IsValidCountry();
        }
    }
}
