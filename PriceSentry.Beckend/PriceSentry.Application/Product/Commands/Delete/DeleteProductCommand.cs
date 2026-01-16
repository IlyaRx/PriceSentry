using MediatR;

namespace PriceSentry.Application.Product.Commands.Delete {
    public class DeleteProductCommand : IRequest {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}
