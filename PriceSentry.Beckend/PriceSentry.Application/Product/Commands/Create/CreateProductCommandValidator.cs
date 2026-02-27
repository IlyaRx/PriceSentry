
using FluentValidation;

namespace PriceSentry.Application.Product.Commands.Create {
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand> {
        public CreateProductCommandValidator() {
            RuleFor(createProductCommand => createProductCommand.UserId).NotEqual(Guid.Empty);
            RuleFor(createProductCommand => createProductCommand.DesiredPrice).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1000000);
            RuleFor(createProductCommand => createProductCommand.ProductUrl).NotEmpty();

        }
    }
}
