using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Clients;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Carts.ClearCart;

internal sealed class ClearCartCommandHandler(
    IApplicationDbContext context,
    CartService cartService) : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand command, CancellationToken cancellationToken)
    {
        IndividualClient? indiviualClient = await context.IndividualClients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ClientId, cancellationToken);
        ProfessionalClient? professionalClient = await context.ProfessionalClients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ClientId, cancellationToken);

        if (indiviualClient is null && professionalClient is null)
        {
            return Result.Failure(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
        }

        await cartService.ClearAsync(command.ClientId, cancellationToken);

        return Result.Success();
    }
}
