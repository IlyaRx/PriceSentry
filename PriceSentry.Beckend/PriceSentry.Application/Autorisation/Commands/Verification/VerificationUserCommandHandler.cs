
using MediatR;
using Microsoft.AspNetCore.Identity;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;

namespace PriceSentry.Application.Autorisation.Commands.Verification {
    public class VerificationUserCommandHandler : IRequestHandler<VerificationUserCommand, string> {
        private readonly IStoregCodeService _storege;
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;

        public VerificationUserCommandHandler(IStoregCodeService storege, UserManager<ApplicationUser> userManager, ITokenService tokenService) {
            _storege = storege;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(VerificationUserCommand request, CancellationToken cancellationToken) {
            var applicationUser = await _userManager.FindByEmailAsync(request.Email);
            if (applicationUser == null || applicationUser.Email != request.Email)
                throw new NotFoundException(nameof(ApplicationUser), request.Email);

            //if (!await _storege.IsValidCodeAsync(request.Email, request.Code, cancellationToken))
            //    throw new InvalidCodeException();

            await _storege.RemoveCodeAsync(request.Email, cancellationToken);

            return await _tokenService.GenerateTokenAsync(applicationUser, cancellationToken);

        }
    }
}
