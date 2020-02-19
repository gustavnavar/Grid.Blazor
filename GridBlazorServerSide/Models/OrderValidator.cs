using FluentValidation;

namespace GridBlazorServerSide.Models
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(p => p.Freight).NotEmpty().WithMessage("You must enter a freight");
        }
    }
}
