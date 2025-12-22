using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.Clients.Create;

internal sealed class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidator()
    {
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(20);
		RuleFor(c => c.LastName).NotEmpty().MaximumLength(20);
	}
}
