
using Application.Abstractions.Messaging;
using Application.Carts;
using Application.Carts.GetCart;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Carts;

internal sealed class GetCart : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("carts/{clientId}", async(
            Guid clientId, 
            IQueryHandler<GetCartQuery, Cart> handler,
            CancellationToken cancellationToken) =>
        {
            Result<Cart> result = await handler.Handle(new GetCartQuery(clientId), cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Carts);

    }
}
