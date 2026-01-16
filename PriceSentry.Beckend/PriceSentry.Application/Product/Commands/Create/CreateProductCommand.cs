using MediatR;
using System.Security.Cryptography.X509Certificates;

namespace PriceSentry.Application.Product.Commands.Create {
    public class CreateProductCommand : IRequest<Guid>{
        public Guid UserId {  get; set; }
        public decimal DesiredPrice { get; set; }
        public required string ProductUrl { get; set; }
    }
}
