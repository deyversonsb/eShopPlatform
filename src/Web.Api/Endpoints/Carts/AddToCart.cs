
using Application.Abstractions.Messaging;
using Application.Carts.AddItemToCart;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Carts;

internal sealed class AddToCart : IEndpoint
{
    public sealed record Request(Guid ClientId, Guid ProductId, decimal Quantity, bool IsProfessionalClient = false);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("carts/add", async (
            Request request, 
            ICommandHandler<AddItemToCartCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddItemToCartCommand(
                request.ClientId,
                request.ProductId,
                request.Quantity,
                request.IsProfessionalClient);

            Result result = await handler.Handle(command, cancellationToken);

			return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithTags(Tags.Carts);
	}
}
