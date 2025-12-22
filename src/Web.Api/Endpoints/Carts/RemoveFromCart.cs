
using Application.Abstractions.Messaging;
using Application.Carts.RemoveItemFromCart;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Carts;

internal sealed class RemoveFromCart : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("carts/{clientId}/remove/{productId}", async (
            Guid clientId,
            Guid productId,
            ICommandHandler<RemoveItemFromCartCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveItemFromCartCommand(clientId, productId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithTags(Tags.Carts);
    }
}
