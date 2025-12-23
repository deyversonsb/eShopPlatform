using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Carts;
using Domain.Clients;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Order.Create;

internal sealed class CreateOrderCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider,
    CartService cartService) : ICommandHandler<CreateOrderCommand>
{
    public async Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        IndividualClient? indiviualClient = await context.IndividualClients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ClientId, cancellationToken);
        ProfessionalClient? professionalClient = await context.ProfessionalClients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ClientId, cancellationToken);

        if (indiviualClient is null && professionalClient is null)
        {
            return Result.Failure(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
        }

        var order = Domain.Orders.Order.Create(command.ClientId, dateTimeProvider.UtcNow);

        Cart cart = await cartService.GetAsync(command.ClientId, cancellationToken);

        if (!cart.Items.Any())
        {
            return Result.Failure(Error.Problem("Carts.Empty", "Cart is empty."));
        }

        foreach (CartItem cartItem in cart.Items)
        {
            Product? product = await context
                .Products
                .FromSql(
                    $"""
                    SELECT id, name, product_type, individual_price, professional_price_greater_than10,professional_price_less_than10
                    FROM public.products
                    WHERE id = {cartItem.ProductId}
                    FOR UPDATE NOWAIT
                    """)
                .SingleOrDefaultAsync(cancellationToken);

            if (product is null)
            {
                return Result.Failure(Error.NotFound("Product.NotFound", $"Product with ID {cartItem.ProductId} was not found."));
            }


            order.AddItem(product, cartItem.Quantity, cartItem.Price);           
        }

        context.Orders.Add(order);

        await context.SaveChangesAsync(cancellationToken);

        await cartService.ClearAsync(command.ClientId, cancellationToken);

        return Result.Success();
    }
}
