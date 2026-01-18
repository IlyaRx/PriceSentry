
using FluentValidation;

namespace PriceSentry.Application.Product.Commands.Delete {
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand> {
        public DeleteProductCommandValidator() { 
            RuleFor(deleteProductCommand => deleteProductCommand.Id).NotEqual(Guid.Empty);
            RuleFor(deleteProductCommand => deleteProductCommand.UserId).NotEqual(Guid.Empty);
        }
    }
}
