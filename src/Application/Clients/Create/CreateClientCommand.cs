using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Clients.Create;

public record CreateClientCommand(
	string FirstName,
	string LastName) : ICommand<Guid>;