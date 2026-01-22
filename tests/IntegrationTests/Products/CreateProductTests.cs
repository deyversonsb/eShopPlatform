using System;
using System.Collections.Generic;
using System.Text;
using Application.Products.Create;
using Domain.Products;
using FluentAssertions;
using IntegrationTests.Abstractions;
using SharedKernel;

namespace IntegrationTests.Products;

public class CreateProductTests : BaseIntegrationTest
{
    public CreateProductTests(IntegrationTestWebAppFactory factory) 
        : base(factory)
    {
    }

    [Fact]
    public async Task Handle_Should_CreateProduct_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateProductCommand(
            Faker.Commerce.ProductName(),
            ProductType.MidRangePhones,
            Faker.Random.Decimal(1m, 100m),
            Faker.Random.Decimal(1m, 50m),
            Faker.Random.Decimal(50m, 100m));

        var handler = new CreateProductCommandHandler(DbContext);

        // Act
        Result<Guid> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_AddProductToDatabase_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateProductCommand(
            Faker.Commerce.ProductName(),
            ProductType.MidRangePhones,
            Faker.Random.Decimal(1m, 100m),
            Faker.Random.Decimal(1m, 50m),
            Faker.Random.Decimal(50m, 100m));

        var handler = new CreateProductCommandHandler(DbContext);

        // Act
        Result<Guid> result = await handler.Handle(command, default);

        // Assert
        Product? product = await DbContext.Products.FindAsync(result.Value);

        product.Should().NotBeNull();
    }
}
