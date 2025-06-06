using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Library.API.Domain;
using Shop.Shared.Models;
using Shop.Shared.Models.ServiceBus;

namespace Shop.Library.API.Features.Books.Commands
{
    public class UpdateBookCommand : IRequest<Guid>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class UpdateBookHandler(BookContext dbContext, IPublishEndpoint publishEndpoint) : IRequestHandler<UpdateBookCommand, Guid>
    {
        public async Task<Guid> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == request.Id);

            if (book == null) throw new AppException("book not found");

            book.Title = request.Title;
            await dbContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new BookUpdated
            {
                Id = book.Id,
                Title = book.Title,
            });

            return book.Id;
        }
    }
}
