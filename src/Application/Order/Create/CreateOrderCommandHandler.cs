using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Order.Create;

internal sealed class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
{
    public Task<Result> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
