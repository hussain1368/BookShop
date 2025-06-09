using AutoMapper;
using Shop.Sales.API.Domain;
using MediatR;
using Shop.Shared;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Shop.Shared.Models.ServiceBus;

namespace Shop.Sales.API.Features.Purchases
{
    public class CreatePurchaseCommand : IRequest<Guid>
    {
        public Guid BookId { get; set; }
        public DateTime PurchaseDate { get; set; }
    }

    public class CreatePurchaseHandler(
            IMapper mapper,
            PurchaseContext dbContext,
            BookProtocol.BookProtocolClient bookClient,
            IPublishEndpoint publishEndpoint) : IRequestHandler<CreatePurchaseCommand, Guid>
    {

        public async Task<Guid> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
        {
            var data = mapper.Map<Purchase>(request);
            var book = await bookClient.GetBookSnapshotAsync(new BookSnapshotRequest { Id = data.BookId.ToString() });

            if (book == null) throw new Exception("Book not found");

            data.BookTitle = book.Title;
            dbContext.Purchases.Add(data);
            await dbContext.SaveChangesAsync();

            var purchaseCount = await dbContext.Purchases.CountAsync(p => p.BookId == request.BookId);
            await publishEndpoint.Publish(new PurchaseAdded
            {
                BookId = request.BookId,
                PurchaseCount = purchaseCount
            });

            return data.Id;
        }
    }
}
