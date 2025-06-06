using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shop.Library.API.Domain;
using Shop.Shared.Models;
using Shop.Shared.Models.ServiceBus;

namespace Shop.Library.API.Features.Books.Consumers
{
    public class PurchaseAddedConsumer(BookContext dbContext) : IConsumer<PurchaseAdded>
    {
        public async Task Consume(ConsumeContext<PurchaseAdded> context)
        {
            var book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == context.Message.BookId);
            if (book == null) throw new AppException("book is not found");
            book.PurchaseCount = context.Message.PurchaseCount;
            await dbContext.SaveChangesAsync();
        }
    }
}
