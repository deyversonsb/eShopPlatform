using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Clients.Update;

public record UpdateClientCommand(
	Guid ClientId,
	string FirstName,
	string LastName) : ICommand;