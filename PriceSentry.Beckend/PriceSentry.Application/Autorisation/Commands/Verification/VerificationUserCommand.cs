using MediatR;

namespace PriceSentry.Application.Autorisation.Commands.Verification {
    public class VerificationUserCommand : IRequest<string>{
        public required string Email { get; set; }
        public required string Code {  get; set; }
    }
}
