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
using Application.Products.Create;
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
    private const decimal IndividualPrice = 2000m;
    private const decimal ProfessionalPriceGreaterThan10 = 1000m;
    private const decimal ProfessionalPriceLessThan10 = 1500m;

    public AddItemToCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _dbContextMock = new();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenIndividualClientDoesNotExist()
    {
        //Arrange
        var command = new AddItemToCartCommand(Guid.NewGuid(), Guid.NewGuid(), Quantity, false);
        var handler = new AddItemToCartCommandHandler(DbContext, CartService);

        //Act
        Result result = await handler.Handle(command, default);

        //Assert
        result.Error.Should().Be(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
    }

    [Fact]
    public async Task IndividualClient_ShouldBeAbleTo_AddItemToCart()
    {
        // Arrange
        var createClientHandler = new CreateClientCommandHandler(DbContext);
        Guid clientId = (await createClientHandler.Handle(new(Faker.Name.FirstName(), Faker.Name.LastName()), default)).Value;

        var createProductHandler = new CreateProductCommandHandler(DbContext);
        Guid productId = (await createProductHandler
            .Handle(new(
                Faker.Commerce.ProductName(),
                ProductType.Laptops,
                IndividualPrice,
                ProfessionalPriceGreaterThan10,
                ProfessionalPriceLessThan10),
                default))
            .Value;

        var command = new AddItemToCartCommand(clientId, productId, Quantity, false);
		var handler = new AddItemToCartCommandHandler(DbContext, CartService);

        // Act
        Result<Cart> result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.ClientId.Should().Be(clientId);

        result.Value
            .Items
            .Should()
            .Contain(i => i.Quantity == Quantity &&
                          i.ProductId == productId &&
                          i.Price == IndividualPrice);
        result.Value
            .TotalPrice.Should().Be(Quantity * IndividualPrice);
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

        Mock<DbSet<ProfessionalClient>> mockProfessionalClient = TestHelpers.GetMockDbSet<ProfessionalClient>(clients);
        Mock<DbSet<Product>> mockProducts = TestHelpers.GetMockDbSet<Product>(products);

        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(mockProfessionalClient.Object);

        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(mockProducts.Object);

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

        Mock<DbSet<ProfessionalClient>> mockProfessionalClient = TestHelpers.GetMockDbSet<ProfessionalClient>(clients);
        Mock<DbSet<Product>> mockProducts = TestHelpers.GetMockDbSet<Product>(products);

        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(mockProfessionalClient.Object);

        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(mockProducts.Object);

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
}
