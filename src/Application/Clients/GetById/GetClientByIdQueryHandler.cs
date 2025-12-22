using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Clients.GetById;

internal sealed class GetClientByIdQueryHandler : IQueryHandler<GetClientByIdQuery, object>
{
    public Task<Result<object>> Handle(GetClientByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
