using MediatR;
using Shop.Sales.API.Features.Purchases;

namespace Shop.Sales.API.Endpoints
{
    public static class PurchasesEndpoints
    {
        public static void MapPurchaseEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/purchase");

            group.MapPost("/", async (IMediator mediator, CreatePurchaseCommand cmd) =>
            {
                var id = await mediator.Send(cmd);
                return Results.Ok(id);
            });
        }
    }
}
