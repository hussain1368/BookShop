using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shop.Library.API.Domain;
using Shop.Shared;

namespace Shop.Library.API.Features
{
    public class BookService(BookContext dbContext) : BookProtocol.BookProtocolBase
    {
        public override async Task<BookSnapshotReply> GetBookSnapshot(BookSnapshotRequest request, ServerCallContext context)
        {
            var book = await dbContext.Books
                .Where(b => b.Id == new Guid(request.Id))
                .Select(b => new BookSnapshotReply
                {
                    Title = b.Title,
                })
                .FirstOrDefaultAsync();

            return book ?? throw new Exception("Book not found");
        }
    }
}
