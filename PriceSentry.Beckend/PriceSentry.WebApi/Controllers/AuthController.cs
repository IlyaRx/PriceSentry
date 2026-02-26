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

        [HttpGet("telegramUrl/{id}")]
        public ActionResult<string> TelegrammUrl(Guid id) {
            return $"https://t.me/AssistantPearl_bot?start={id}";
        }

        [HttpPost("login")]
        public async Task<ActionResult<Guid>> Login([FromBody] LoginRequestDto request) {
            var command = new RegistrUserCommand() { Email = request.Email};
            var userId = await _mediator.Send(command);

            return Ok(new { 
                message = "Код отпарвылен на почту",
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
}
