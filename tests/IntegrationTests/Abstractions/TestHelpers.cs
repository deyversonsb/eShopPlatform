using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Application.Clients.Create;
using Application.Products.Create;
using Bogus;
using Domain.Products;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedKernel;

namespace IntegrationTests.Abstractions;

internal static class TestHelpers
{
    internal static Mock<DbSet<TEntity>> GetMockDbSet<TEntity>(IQueryable<TEntity> entities)
        where TEntity : Entity
    {
        var mockDbSet = new Mock<DbSet<TEntity>>();
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.Provider).Returns(entities.Provider);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.Expression).Returns(entities.Expression);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.ElementType).Returns(entities.ElementType);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());

        return mockDbSet;
    }

    internal static async Task CreateClientAsync(
        this CreateClientCommandHandler handle)
    {
        var faker = new Faker();

        var command = new CreateClientCommand(faker.Name.FirstName(), faker.Name.LastName());

        Result<Guid> result = await handle.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
    }

    internal static async Task CreateProductAsync(
        this CreateProductCommandHandler handle,
        ProductType productType = ProductType.Laptops,
        decimal price = 50m)
    {
        var faker = new Faker();

        var command = new CreateProductCommand(
            faker.Commerce.ProductName(),
            faker.PickRandom<ProductType>(),
            faker.Random.Decimal(1, price),
            faker.Random.Decimal(1, price / 2),
            faker.Random.Decimal(price / 2, price));

        Result<Guid> result = await handle.Handle(command, default);

        result.IsSuccess.Should().BeTrue();
    }
}
