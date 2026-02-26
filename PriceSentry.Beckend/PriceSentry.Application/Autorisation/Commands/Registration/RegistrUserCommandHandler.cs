using MediatR;
using Microsoft.AspNetCore.Identity;
using PriceSentry.Application.Interfaces;
using PriceSentry.Application.Interfaces.Notifications;
using PriceSentry.Domain;

namespace PriceSentry.Application.Autorisation.Commands.Registration {
    public class RegistrUserCommandHandler : IRequestHandler<RegistrUserCommand, Guid> {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStoregCodeService _storeg;
        private readonly IGeneratedCode _generatedCode;
        private readonly IUserCodeNotificationService _userNotificationService;
        public RegistrUserCommandHandler(IStoregCodeService storeg,
                                         IGeneratedCode generatedCode,
                                         IUserCodeNotificationService userNotificationService,
                                         UserManager<ApplicationUser> userManager) {
            _storeg = storeg;
            _generatedCode = generatedCode;
            _userNotificationService = userNotificationService;
            _userManager = userManager;
        }


        public async Task<Guid> Handle(RegistrUserCommand request, CancellationToken cancellationToken) {
            var applicationUser = await _userManager.FindByEmailAsync(request.Email);
            if (applicationUser == null) {
                applicationUser = new ApplicationUser() { UserName = request.Email, Email = request.Email };
                await _userManager.CreateAsync(applicationUser);
            }

            await Task.Delay(500, cancellationToken);

            var code = _generatedCode.GetCode();
            await _storeg.StoreCodeAsync(request.Email, code, cancellationToken);
            await _userNotificationService.SendCodeNotificationAsync(request.Email, code);

            return applicationUser.Id;
        }
    }
}
