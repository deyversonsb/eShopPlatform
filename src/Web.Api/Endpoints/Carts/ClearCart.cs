
using Application.Abstractions.Messaging;
using Application.Carts.ClearCart;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Carts;

internal sealed class ClearCart : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("carts/{clientId}", async (
            Guid clientId,
            ICommandHandler<ClearCartCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ClearCartCommand(clientId);
            Result result = await handler.Handle(command, cancellationToken);
            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithTags(Tags.Carts);
    }
}
