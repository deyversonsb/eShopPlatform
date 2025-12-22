using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Clients.GetById;

public record GetClientByIdQuery(Guid ClientId) : IQuery<object>;
