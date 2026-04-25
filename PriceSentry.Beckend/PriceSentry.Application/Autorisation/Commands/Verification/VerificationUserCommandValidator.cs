using FluentValidation;

namespace PriceSentry.Application.Autorisation.Commands.Verification {
    public class VerificationUserCommandValidator : AbstractValidator<VerificationUserCommand> {
        public VerificationUserCommandValidator() {
            RuleFor(x => x.Email).NotEmpty().Matches(@"^(?!.*\.\.)[^@\s]+@([^@\s]+\.)+[^@\s]+$");
            RuleFor(x => x.Code).NotEmpty();
        }
    }
}