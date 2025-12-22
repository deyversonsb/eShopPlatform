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

        var mockClientSet = new Mock<DbSet<IndividualClient>>();
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Provider).Returns(clients.Provider);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Expression).Returns(clients.Expression);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.ElementType).Returns(clients.ElementType);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.GetEnumerator()).Returns(clients.GetEnumerator());

        var mockProfessionalSet = new Mock<DbSet<ProfessionalClient>>();
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.Provider).Returns(professionalClients.Provider);
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.Expression).Returns(professionalClients.Expression);
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.ElementType).Returns(professionalClients.ElementType);
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.GetEnumerator()).Returns(professionalClients.GetEnumerator());

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(mockClientSet.Object);
        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(mockProfessionalSet.Object);

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

        var mockClientSet = new Mock<DbSet<IndividualClient>>();
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Provider).Returns(clients.Provider);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.Expression).Returns(clients.Expression);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.ElementType).Returns(clients.ElementType);
        mockClientSet.As<IQueryable<IndividualClient>>()
            .Setup(m => m.GetEnumerator()).Returns(clients.GetEnumerator());

        var mockProfessionalSet = new Mock<DbSet<ProfessionalClient>>();
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.Provider).Returns(professionalClients.Provider);
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.Expression).Returns(professionalClients.Expression);
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.ElementType).Returns(professionalClients.ElementType);
        mockProfessionalSet.As<IQueryable<ProfessionalClient>>()
            .Setup(m => m.GetEnumerator()).Returns(professionalClients.GetEnumerator());

        _dbContextMock.Setup(db => db.IndividualClients).ReturnsDbSet(mockClientSet.Object);
        _dbContextMock.Setup(db => db.ProfessionalClients).ReturnsDbSet(mockProfessionalSet.Object);

        var command = new ClearCartCommand(client.Id);

        var handler = new ClearCartCommandHandler(_dbContextMock.Object, CartService);

        //Act
        Result result = await handler.Handle(command, default);

        //Assert
        result.IsSuccess.Should().BeTrue();
    }
}
