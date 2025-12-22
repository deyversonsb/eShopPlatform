using System;
using System.Collections.Generic;
using System.Text;
using SharedKernel;

namespace Domain.Products;

public sealed class Product : Entity
{
    private Product(
        Guid id,
        string name,
        ProductType productType,
        decimal individualPrice,
        decimal professionalPriceGreaterThan10,
		decimal professionalPriceLessThan10) : base(id)
	{ 
        Name = name;
        ProductType = productType;
        IndividualPrice = individualPrice;
		ProfessionalPriceGreaterThan10 = professionalPriceGreaterThan10;
        ProfessionalPriceLessThan10 = professionalPriceLessThan10;
	}
	public string Name { get; private set; }
    public ProductType ProductType { get; private set; }
    public decimal IndividualPrice { get; private set; }
	public decimal ProfessionalPriceGreaterThan10 { get; private set; }
	public decimal ProfessionalPriceLessThan10 { get; private set; }
	public static Product Create(
        string name,
        ProductType productType,
        decimal individualPrice,
        decimal professionalPriceGreaterThan10,
        decimal professionalPriceLessThan10)
    => new(Guid.NewGuid(), name, productType, individualPrice, professionalPriceGreaterThan10, professionalPriceLessThan10);
}
