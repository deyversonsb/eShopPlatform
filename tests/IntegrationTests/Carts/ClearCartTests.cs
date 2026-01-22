using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Carts.AddItemToCart;
using Application.Carts.ClearCart;
using Application.Clients.Create;
using Application.Products.Create;
using Domain.Clients;
using Domain.Products;
using FluentAssertions;
using IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace IntegrationTests.Carts;
public class ClearCartTests : BaseIntegrationTest
{
    public ClearCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenClientDoesNotExist()
    {
        //Arrange
        var createClientHandler = new CreateClientCommandHandler(DbContext);
        await createClientHandler.CreateClientAsync();

        var command = new ClearCartCommand(Guid.NewGuid());

        var handler = new ClearCartCommandHandler(DbContext, CartService);

        //Act
        Result result = await handler.Handle(command, default);

        //Assert
        result.Error.Should().Be(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenClientExists()
    {
        //Arrange
        var createClientHandler = new CreateClientCommandHandler(DbContext);
        Guid clientId = (await createClientHandler.Handle(new(Faker.Name.FirstName(), Faker.Name.LastName()), default)).Value;

        var command = new ClearCartCommand(clientId);

        var handler = new ClearCartCommandHandler(DbContext, CartService);

        //Act
        Result result = await handler.Handle(command, default);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}
