using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Order.Create;

public record CreateOrderCommand(Guid ClientId) : ICommand;
