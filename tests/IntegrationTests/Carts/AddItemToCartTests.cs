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

        var mockClientSet = new Mock<DbSet<IndividualClient>>();
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Provider).Returns(clients.Provider);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Expression).Returns(clients.Expression);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.ElementType).Returns(clients.ElementType);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.GetEnumerator()).Returns(clients.GetEnumerator());

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(mockClientSet.Object);

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

        var mockClientSet = new Mock<DbSet<IndividualClient>>();
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Provider).Returns(clients.Provider);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Expression).Returns(clients.Expression);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.ElementType).Returns(clients.ElementType);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.GetEnumerator()).Returns(clients.GetEnumerator());

        var mockProductSet = new Mock<DbSet<Product>>();
        mockProductSet.As<IQueryable<Product>>()
            .Setup(m => m.Provider).Returns(products.Provider);
        mockProductSet.As<IQueryable<Product>>()
            .Setup(m => m.Expression).Returns(products.Expression);
        mockProductSet.As<IQueryable<Product>>()
            .Setup(m => m.ElementType).Returns(products.ElementType);
        mockProductSet.As<IQueryable<Product>>()
            .Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(mockClientSet.Object);

        _dbContextMock.Setup(db => db.Products).ReturnsDbSet(mockProductSet.Object);

        var command = new AddItemToCartCommand(client.Id, product.Id, Quantity, false);

		var handler = new AddItemToCartCommandHandler(_dbContextMock.Object, CartService);

        // Act
        Result result = await handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

}
