using FluentValidation;
using FluentValidation.Validators;

namespace PriceSentry.Application.Autorisation.Commands.Registration;

public class RegistrUserCommandValidator : AbstractValidator<RegistrUserCommand> {
    public RegistrUserCommandValidator() {
        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(@"^(?!.*\.\.)[^@\s]+@([^@\s]+\.)+[^@\s]+$");
    }
}