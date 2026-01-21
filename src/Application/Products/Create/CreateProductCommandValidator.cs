using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Application.Products.Create;

internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ProductType).IsInEnum();
        RuleFor(x => x.IndividualPrice).NotEmpty();
        RuleFor(x => x.ProfessionalPriceGreaterThan10).NotEmpty();
        RuleFor(x => x.ProfessionalPriceLessThan10).NotEmpty();
    }
}
