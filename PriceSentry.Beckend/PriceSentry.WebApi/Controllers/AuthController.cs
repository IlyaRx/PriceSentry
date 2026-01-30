using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PriceSentry.Application.Autorisation.Commands.Registration;
using PriceSentry.Application.Autorisation.Commands.Verification;
using PriceSentry.Domain;
using PriceSentry.WebApi.Models;
using Telegram.Bot.Types;

namespace PriceSentry.WebApi.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Guid>> Login([FromBody] LoginRequestDto request) {
            var command = new RegistrUserCommand() { Email = request.Email};
            var userId = await _mediator.Send(command);

            return Ok(new { 
                message = "Код отпарвылен нв почту",
                userId
            });
        }

        [HttpPost("verify")]
        public async Task<ActionResult<AuthResponse>> Veryfy([FromBody] VerifyRequestDto request) {
            var command = new VerificationUserCommand() {
                Code = request.Code,
                Email = request.Email,
            };

            var tocen = await _mediator.Send(command);

            return Ok(new AuthResponse {
                Token = tocen,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Message = "Успешная аутентификация"
            });
        }

        }
        // Простой тест-контроллер для UserManager
        [ApiController]
        [Route("api/test")]
        public class TestController : ControllerBase {
            private readonly UserManager<ApplicationUser> _userManager;

            public TestController(UserManager<ApplicationUser> userManager) {
                _userManager = userManager;
            }

            [HttpGet("check")]
            public IActionResult CheckInjection() {
                // Если не падает с ошибкой - UserManager внедрен!
                return Ok(new { Message = "UserManager injected successfully" });
            }

            [HttpPost("create")]
            public async Task<IActionResult> CreateTestUser() {
                var user = new ApplicationUser {
                    Email = "test@test.com",
                    UserName = "test@test.com",
                    TelegramChatId = 123456789
                };

                var result = await _userManager.CreateAsync(user, "Password123!");
                return result.Succeeded ? Ok() : BadRequest(result.Errors);
            }
    }
}
