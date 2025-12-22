using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Carts.ClearCart;

public sealed record ClearCartCommand(Guid ClientId) : ICommand;
