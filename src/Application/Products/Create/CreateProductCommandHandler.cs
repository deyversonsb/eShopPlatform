using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Products;
using SharedKernel;

namespace Application.Products.Create;

internal sealed class CreateProductCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            command.Name,
            command.ProductType,
            command.IndividualPrice,
            command.ProfessionalPriceGreaterThan10,
            command.ProfessionalPriceLessThan10);

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
