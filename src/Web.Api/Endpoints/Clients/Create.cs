using Application.Abstractions.Messaging;
using Application.Clients.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Clients;

internal sealed class Create : IEndpoint
{
	public sealed record Request(string FirstName, string LastName);
	public void MapEndpoint(IEndpointRouteBuilder app)
	{
		app.MapPost("clients", async (
			Request request,
			ICommandHandler<CreateClientCommand, Guid> handler,
			CancellationToken cancellationToken) =>
		{
			var command = new CreateClientCommand(request.FirstName, request.LastName);

			Result<Guid> result = await handler.Handle(command, cancellationToken);

			return result.Match(Results.Ok, CustomResults.Problem);
		})
		.WithTags(Tags.Clients);
	}
}