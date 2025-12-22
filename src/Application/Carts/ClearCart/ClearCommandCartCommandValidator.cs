using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.Carts.ClearCart;

internal sealed class ClearCartCommandValidator : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(c => c.ClientId).NotEmpty();
    }
}
