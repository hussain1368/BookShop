using Shop.Library.API.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shop.Library.API.Features.Books;

namespace Shop.Library.API.Endpoints
{
    public static class BooksEndpoints
    {
        public static void MapBookEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/book");

            group.MapPost("/", async (IMediator mediator, CreateBookCommand cmd) =>
            {
                var id = await mediator.Send(cmd);
                return Results.Ok(id);
            });

            group.MapPut("/", async (IMediator mediator, UpdateBookCommand cmd) =>
            {
                var id = await mediator.Send(cmd);
                return Results.Ok(id);
            });

            group.MapGet("/{id}", async (BookContext dbContext, Guid id) =>
            {
                var book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
                return book;
            });

            group.MapGet("/", async (BookContext dbContext) =>
            {
                var books = await dbContext.Books.ToListAsync();
                return books;
            });
        }
    }
}
