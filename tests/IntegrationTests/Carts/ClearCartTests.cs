using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Carts.AddItemToCart;
using Application.Carts.ClearCart;
using Domain.Clients;
using FluentAssertions;
using IntegrationTests.Abstractions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using SharedKernel;

namespace IntegrationTests.Carts;
public class ClearCartTests : BaseIntegrationTest
{
    private readonly Mock<IApplicationDbContext> _dbContextMock;
    public ClearCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
        _dbContextMock = new();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenClientDoesNotExist()
    {
        //Arrange
        var client = IndividualClient.Create(
                Faker.Name.FirstName(),
                Faker.Name.LastName());

        IQueryable<IndividualClient> clients = new List<IndividualClient>
        {
            IndividualClient.Create(Faker.Name.FirstName(), Faker.Name.LastName())
        }.AsQueryable();

        IQueryable<ProfessionalClient> professionalClients = new List<ProfessionalClient>
        {
            ProfessionalClient.Create(Faker.Company.CompanyName(), "VATNumber", "BusinessNumber", 10_000m)
        }.AsQueryable();

        Mock<DbSet<IndividualClient>> mockIndividualClient = TestHelpers.GetMockDbSet<IndividualClient>(clients);
        Mock<DbSet<ProfessionalClient>> mockProfessionalClient = TestHelpers.GetMockDbSet<ProfessionalClient>(professionalClients);

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(mockIndividualClient.Object);
        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(mockProfessionalClient.Object);

        var command = new ClearCartCommand(client.Id);

        var handler = new ClearCartCommandHandler(_dbContextMock.Object, CartService);

        //Act
        Result result = await handler.Handle(command, default);

        //Assert
        result.Error.Should().Be(Error.NotFound("Client.NotFound", $"Client with ID {command.ClientId} was not found."));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenClientExists()
    {
        //Arrange
        var client = IndividualClient.Create(
                Faker.Name.FirstName(),
                Faker.Name.LastName());

        IQueryable<IndividualClient> clients = new List<IndividualClient>
        {
            client
        }.AsQueryable();

        IQueryable<ProfessionalClient> professionalClients = new List<ProfessionalClient>
        {
            ProfessionalClient.Create(Faker.Company.CompanyName(), "VATNumber", "BusinessNumber", 10_000m)
        }.AsQueryable();

        Mock<DbSet<IndividualClient>> mockIndividualClient = TestHelpers.GetMockDbSet<IndividualClient>(clients);
        Mock<DbSet<ProfessionalClient>> mockProfessionalClient = TestHelpers.GetMockDbSet<ProfessionalClient>(professionalClients);

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(mockIndividualClient.Object);
        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(mockProfessionalClient.Object);

        var command = new ClearCartCommand(client.Id);

        var handler = new ClearCartCommandHandler(_dbContextMock.Object, CartService);

        //Act
        Result result = await handler.Handle(command, default);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}
