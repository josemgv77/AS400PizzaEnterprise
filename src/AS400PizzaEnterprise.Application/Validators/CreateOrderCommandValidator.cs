using AS400PizzaEnterprise.Application.Commands.Orders;
using FluentValidation;

namespace AS400PizzaEnterprise.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required")
            .MaximumLength(200)
            .WithMessage("Street must not exceed 200 characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(100)
            .WithMessage("City must not exceed 100 characters");

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required")
            .MaximumLength(50)
            .WithMessage("State must not exceed 50 characters");

        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("ZipCode is required")
            .MaximumLength(20)
            .WithMessage("ZipCode must not exceed 20 characters");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .MaximumLength(100)
            .WithMessage("Country must not exceed 100 characters");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Order must contain at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.PizzaId)
                .NotEmpty()
                .WithMessage("Pizza ID is required");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero");
        });
    }
}
