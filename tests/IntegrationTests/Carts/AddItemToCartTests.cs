using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using Application.Abstractions.Data;
using Application.Carts;
using Application.Carts.AddItemToCart;
using Application.Clients.Create;
using Castle.Core.Resource;
using Domain.Clients;
using Domain.Products;
using FluentAssertions;
using Infrastructure.Database;
using IntegrationTests.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Moq.EntityFrameworkCore;
using SharedKernel;

namespace IntegrationTests.Carts;

public sealed class AddItemToCartTests : BaseIntegrationTest
{
    private readonly Mock<IApplicationDbContext> _dbContextMock;

    public const decimal Quantity = 10;

    public AddItemToCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _dbContextMock = new();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenIndividualClientDoesNotExist()
    {
        //Arrange
        var client = IndividualClient.Create(
                Faker.Name.FirstName(),
                Faker.Name.LastName());

        IQueryable<IndividualClient> clients = new List<IndividualClient>
        {
            IndividualClient.Create(Faker.Name.FirstName(), Faker.Name.LastName())
        }.AsQueryable();

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(GetMockDbSet<IndividualClient>(clients).Object);

        var command = new AddItemToCartCommand(client.Id, Guid.NewGuid(), Quantity, false);

        var handler = new AddItemToCartCommandHandler(_dbContextMock.Object, CartService);

        //Act
        Result result = await handler.Handle(command, default);

        //Assert
        result.Error.Should().Be(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
    }

    [Fact]
	public async Task IndividualClient_ShouldBeAbleTo_AddItemToCart()
	{
        // Arrange
        var client = IndividualClient.Create(
                Faker.Name.FirstName(), 
                Faker.Name.LastName());

        var product = Product.Create(
           Faker.Commerce.ProductName(),
           ProductType.Laptops,
           2000m,
           1000m,
           1500m);

        IQueryable<IndividualClient> clients = new List<IndividualClient>
        {
            client
        }.AsQueryable();

        IQueryable<Product> products = new List<Product>
        {
            product
        }.AsQueryable();

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(GetMockDbSet<IndividualClient>(clients).Object);

        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(GetMockDbSet<Product>(products).Object);

        var command = new AddItemToCartCommand(client.Id, product.Id, Quantity, false);

		var handler = new AddItemToCartCommandHandler(_dbContextMock.Object, CartService);

        // Act
        Result<Cart> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.ClientId.Should().Be(client.Id);

        result.Value
            .Items
            .Should()
            .Contain(i => i.Quantity == Quantity &&
                          i.ProductId == product.Id &&
                          i.Price == product.IndividualPrice);
        result.Value
            .TotalPrice.Should().Be(Quantity * product.IndividualPrice);
    }

    [Fact]
    public async Task ProfessionalClient_ShouldBeAbleTo_AddItemToCart_WhenAnnualRevenue_GreaterThan10M()
    {
        // Arrange
        var client = ProfessionalClient.Create(
                Faker.Company.CompanyName(),
                "VAT0123456",
                "BRN0123456",
                10_500M);

        var product = Product.Create(
           Faker.Commerce.ProductName(),
           ProductType.Laptops,
           2000m,
           1000m,
           1500m);

        IQueryable<ProfessionalClient> clients = new List<ProfessionalClient>
        {
            client
        }.AsQueryable();

        IQueryable<Product> products = new List<Product>
        {
            product
        }.AsQueryable();

        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(GetMockDbSet<ProfessionalClient>(clients).Object);

        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(GetMockDbSet<Product>(products).Object);

        var command = new AddItemToCartCommand(client.Id, product.Id, Quantity, true);

        var handler = new AddItemToCartCommandHandler(_dbContextMock.Object, CartService);

        // Act
        Result<Cart> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.ClientId.Should().Be(client.Id);

        result.Value
            .Items
            .Should()
            .Contain(i => i.Quantity == Quantity &&
                          i.ProductId == product.Id &&
                          i.Price == product.ProfessionalPriceGreaterThan10);
        result.Value
            .TotalPrice.Should().Be(Quantity * product.ProfessionalPriceGreaterThan10);
    }

    [Fact]
    public async Task ProfessionalClient_ShouldBeAbleTo_AddItemToCart_WhenAnnualRevenue_LessThan10M()
    {
        // Arrange
        var client = ProfessionalClient.Create(
                Faker.Company.CompanyName(),
                "VAT0123456",
                "BRN0123456",
                5_000M);

        var product = Product.Create(
           Faker.Commerce.ProductName(),
           ProductType.Laptops,
           2000m,
           1000m,
           1500m);

        IQueryable<ProfessionalClient> clients = new List<ProfessionalClient>
        {
            client
        }.AsQueryable();

        IQueryable<Product> products = new List<Product>
        {
            product
        }.AsQueryable();

        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(GetMockDbSet<ProfessionalClient>(clients).Object);

        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(GetMockDbSet<Product>(products).Object);

        var command = new AddItemToCartCommand(client.Id, product.Id, Quantity, true);

        var handler = new AddItemToCartCommandHandler(_dbContextMock.Object, CartService);

        // Act
        Result<Cart> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.ClientId.Should().Be(client.Id);

        result.Value
            .Items
            .Should()
            .Contain(i => i.Quantity == Quantity &&
                          i.ProductId == product.Id &&
                          i.Price == product.ProfessionalPriceLessThan10);
        result.Value
            .TotalPrice.Should().Be(Quantity * product.ProfessionalPriceLessThan10);
    }

    internal static Mock<DbSet<TEntity>> GetMockDbSet<TEntity>(
        IQueryable<TEntity> queryable) 
        where TEntity : class
    {
        var mockDbSet = new Mock<DbSet<TEntity>>();
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.Provider).Returns(queryable.Provider);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        return mockDbSet;
    }
}
