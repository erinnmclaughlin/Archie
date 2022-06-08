using FluentValidation;

namespace Archie.Shared.Features.WorkOrders.Create;

public class CreateWorkOrderRequest
{
    public long CustomerId { get; set; }
    public DateTime? DueDate { get; set; }

    public CreateWorkOrderRequest(long customerId, DateTime? dueDate)
    {
        CustomerId = customerId;
        DueDate = dueDate;
    }

    public void Validate() => new Validator().ValidateAndThrow(this);

    public class Validator : AbstractValidator<CreateWorkOrderRequest>
    {
        public Validator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer is required.");
        }
    }

}
