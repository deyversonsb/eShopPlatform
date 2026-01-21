using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;
using Domain.Products;

namespace Application.Products.Create;

public record CreateProductCommand(
    string Name,  
    ProductType ProductType,
    decimal IndividualPrice,
    decimal ProfessionalPriceGreaterThan10,
    decimal ProfessionalPriceLessThan10) : ICommand<Guid>;
