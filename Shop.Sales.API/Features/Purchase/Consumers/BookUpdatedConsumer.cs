using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shop.Sales.API.Domain;
using Shop.Shared.Models.ServiceBus;

namespace Shop.Sales.API.Features.Purchases.Consumers
{
    public class BookUpdatedConsumer(PurchaseContext dbContext) : IConsumer<BookUpdated>
    {
        public async Task Consume(ConsumeContext<BookUpdated> context)
        {
            await dbContext.Purchases
                .Where(p => p.BookId == context.Message.Id)
                .ExecuteUpdateAsync(p => p.SetProperty(p1 => p1.BookTitle, context.Message.Title));
        }
    }
}
