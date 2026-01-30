
using MediatR;

namespace PriceSentry.Application.Autorisation.Commands.Registration {
    public class RegistrUserCommand : IRequest<Guid> {
        public required string Email { get; set; }
    }
}
