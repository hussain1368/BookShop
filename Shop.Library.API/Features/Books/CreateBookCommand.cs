using AutoMapper;
using Shop.Library.API.Domain;
using MediatR;

namespace Shop.Library.API.Features.Books
{
    public class CreateBookCommand : IRequest<Guid>
    {
        public Guid AuthorId { get; set; }
        public Guid PublisherId { get; set; }
        public Guid CategoryId { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Description { get; set; }
    }

    public class CreateBookHandler(BookContext dbContext, IMapper mapper) : IRequestHandler<CreateBookCommand, Guid>
    {
        public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var book = mapper.Map<Book>(request);
            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync(cancellationToken);

            return book.Id;
        }
    }
}
