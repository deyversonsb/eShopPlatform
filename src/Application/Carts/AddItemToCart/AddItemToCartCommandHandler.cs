using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Clients;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Carts.AddItemToCart;

internal sealed class AddItemToCartCommandHandler(
    IApplicationDbContext context,
    CartService cartService) : ICommandHandler<AddItemToCartCommand>
{
    public async Task<Result> Handle(AddItemToCartCommand command, CancellationToken cancellationToken)
    {
        ProfessionalClient? professionalClient = null;
		IndividualClient? individualClient = null;

		if (command.IsProfessionalClient)
        {
			professionalClient = await context.ProfessionalClients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == command.ClientId, cancellationToken);
		}
        else
        {
			individualClient = await context.IndividualClients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == command.ClientId, cancellationToken);			
		}

		if (professionalClient is null && individualClient is null)
		{
			return Result.Failure(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
		}

        Product? product = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == command.ProductId, cancellationToken);

        if (product is null)
        {
			return Result.Failure(Error.NotFound("Product.NotFound", $"Product with ID {command.ProductId} was not found."));
		}

        var cartItem = new CartItem
        {
            ProductId = command.ProductId,
            Quantity = command.Quantity,
            Price = !command.IsProfessionalClient
                    ? product.IndividualPrice
					: SetPriceForProfessionalClient(product!, professionalClient!.AnnualRevenue)
		};

        await cartService.AddItemAsync(command.ClientId, cartItem, cancellationToken);

		return Result.Success();
    }
    internal static decimal SetPriceForProfessionalClient(Product product, decimal annualRevenue) =>
        annualRevenue switch
        {
            > 10_000m => product?.ProfessionalPriceGreaterThan10 ?? 0m,
			_ => product?.ProfessionalPriceLessThan10 ?? 0m
		};
}
