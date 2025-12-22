using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Clients;
using SharedKernel;

namespace Application.Clients.Create;

internal sealed class CreateClientCommandHandler(
    IApplicationDbContext context) : ICommandHandler<CreateClientCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateClientCommand command, CancellationToken cancellationToken)
    {
        var individualClient = IndividualClient.Create(command.FirstName, command.LastName);

        context.IndividualClients.Add(individualClient);

        await context.SaveChangesAsync(cancellationToken);

        return individualClient.Id;
    }
}
