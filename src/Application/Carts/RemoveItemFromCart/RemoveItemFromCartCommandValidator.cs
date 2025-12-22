using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.Carts.RemoveItemFromCart;

internal sealed class RemoveItemFromCartCommandValidator : AbstractValidator<RemoveItemFromCartCommand>
{
    public RemoveItemFromCartCommandValidator()
    {
        RuleFor(c => c.ClientId).NotEmpty();
        RuleFor(c => c.ProductId).NotEmpty();
	}
}
