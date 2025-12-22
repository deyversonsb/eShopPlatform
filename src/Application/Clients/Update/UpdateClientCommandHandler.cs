using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Clients.Update;

internal sealed class UpdateClientCommandHandler : ICommandHandler<UpdateClientCommand>
{
    public Task<Result> Handle(UpdateClientCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
