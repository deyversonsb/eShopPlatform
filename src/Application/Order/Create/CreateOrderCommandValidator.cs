using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.Order.Create;

internal sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(o => o.ClientId).NotEmpty();
    }
}
