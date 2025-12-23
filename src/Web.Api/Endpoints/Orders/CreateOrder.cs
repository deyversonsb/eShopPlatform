
using Application.Abstractions.Messaging;
using Application.Order.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Orders;

internal sealed class CreateOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("orders/add/{clientId}", async (
            Guid clientId,
            ICommandHandler<CreateOrderCommand> handler,
            CancellationToken cancellationToken) =>
        {
            Result result = await handler.Handle(new CreateOrderCommand(clientId), cancellationToken);

            return result.Match(() => Results.Ok(), CustomResults.Problem);
        })
        .WithTags("Orders");
    }
}
