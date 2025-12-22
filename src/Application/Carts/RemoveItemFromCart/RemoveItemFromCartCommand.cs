using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Carts.RemoveItemFromCart;

public sealed record RemoveItemFromCartCommand(Guid ClientId, Guid ProductId) : ICommand;
