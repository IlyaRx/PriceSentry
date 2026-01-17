using MediatR;

namespace PriceSentry.Application.Product.Commands.Update {
    public class UpdateProductCommand : IRequest {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal DesiredPrice { get; set; }
    }
}
