using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.Clients.Update;

internal sealed class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidator()
    {
        RuleFor(c => c.ClientId).NotEmpty();
        RuleFor(c => c.FirstName).NotEmpty().MaximumLength(20);
		RuleFor(c => c.LastName).NotEmpty().MaximumLength(20);
	}
}
