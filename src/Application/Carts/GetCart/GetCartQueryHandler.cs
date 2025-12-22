using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Carts.GetCart;

internal sealed class GetCartQueryHandler(
    CartService cartService) : IQueryHandler<GetCartQuery, Cart>
{
    public async Task<Result<Cart>> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        return await cartService.GetAsync(query.ClientId, cancellationToken);
	}
}
