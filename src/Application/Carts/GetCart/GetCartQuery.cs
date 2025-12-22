using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Carts.GetCart;

public record GetCartQuery(Guid ClientId) : IQuery<Cart>;
