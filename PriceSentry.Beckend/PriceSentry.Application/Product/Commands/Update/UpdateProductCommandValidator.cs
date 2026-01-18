
using FluentValidation;

namespace PriceSentry.Application.Product.Commands.Update {
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand> {
        public UpdateProductCommandValidator() {
            RuleFor(updateProductCommand => updateProductCommand.Id).NotEqual(Guid.Empty);
            RuleFor(updateProductCommand => updateProductCommand.UserId).NotEqual(Guid.Empty);
            RuleFor(updateProductCommand => updateProductCommand.DesiredPrice).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1000000);
        }
    }
}
