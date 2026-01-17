using MediatR;
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces.cs;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Commands.Update {
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>{

        private readonly IPriceSentryDbContext _dbContext;

        public UpdateProductCommandHandler(IPriceSentryDbContext dbContext) =>  _dbContext = dbContext;

        public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken) {
            var entity = await _dbContext.Products.FindAsync(new object[] {request.Id}, cancellationToken);

            if (entity == null || request.UserId != entity.UserId) {
                throw new NotFoundException(nameof(TrackingProduct), request);
            }

            entity.DesiredPrice = request.DesiredPrice;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
