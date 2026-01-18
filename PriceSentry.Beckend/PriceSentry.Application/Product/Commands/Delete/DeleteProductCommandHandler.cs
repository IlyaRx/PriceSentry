using MediatR;
using Microsoft.EntityFrameworkCore;
using PriceSentry.Application.Common.Exceptions;
using PriceSentry.Application.Interfaces;
using PriceSentry.Domain;

namespace PriceSentry.Application.Product.Commands.Delete {
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand> {

        private readonly IPriceSentryDbContext _dbContext;

        public DeleteProductCommandHandler(IPriceSentryDbContext dbContext) =>
            _dbContext = dbContext;
        

        public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken) {
            var entity = await _dbContext.Products.FindAsync(new object[] {request.Id}, cancellationToken);

            if(entity == null || request.UserId != entity.UserId) {
                throw new NotFoundException(nameof(TrackingProduct), request);
            }

            _dbContext.Products.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
