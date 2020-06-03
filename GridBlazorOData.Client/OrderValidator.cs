using FluentValidation;
using GridBlazorOData.Shared.Models;

namespace GridBlazorOData.Client
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
