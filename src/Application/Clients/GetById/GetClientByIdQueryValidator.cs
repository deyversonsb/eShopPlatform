using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.Clients.GetById;

internal sealed class GetClientByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
{
    public GetClientByIdQueryValidator()
    {
        RuleFor(c => c.ClientId).NotEmpty();
    }
}
