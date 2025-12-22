using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Clients;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandHandler(
    IApplicationDbContext context,
    CartService cartService) : ICommandHandler<RemoveItemFromCartCommand>
{
    public async Task<Result> Handle(RemoveItemFromCartCommand command, CancellationToken cancellationToken)
    {
        ProfessionalClient? professionalClient = await context.ProfessionalClients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == command.ClientId, cancellationToken);
        IndividualClient? individualClient = await context.IndividualClients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == command.ClientId, cancellationToken);

		if (professionalClient is null && individualClient is null)
        {
            return Result.Failure(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
		}

        Product? product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken);

		if (product is null)
		{
			return Result.Failure(Error.NotFound("Product.NotFound", $"Product with ID {command.ProductId} was not found."));
		}

        await cartService.RemoveItemAsync(command.ClientId, command.ProductId, cancellationToken);

		return Result.Success();
	}
}
