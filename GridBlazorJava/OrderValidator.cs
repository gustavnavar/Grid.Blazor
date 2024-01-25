using FluentValidation;
using GridBlazorJava.Models;

namespace GridBlazorJava
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(p => p.Freight).NotEmpty().WithMessage("You must enter a freight");
            RuleFor(p => p.OrderDate).NotEmpty().WithMessage("You must enter an order date");
        }
    }
}
