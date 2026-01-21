using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Application.Clients.Create;
using Domain.Clients;
using FluentAssertions;
using FluentValidation.TestHelper;
using IntegrationTests.Abstractions;
using Moq;
using SharedKernel;

namespace IntegrationTests.Clients;

public class CreateClientTests : BaseIntegrationTest
{
    private readonly CreateClientCommandValidator validator;
    public CreateClientTests(IntegrationTestWebAppFactory factory)
        :base(factory)
    {
        validator = new CreateClientCommandValidator();
    }

    [Fact]
    public async Task Should_CreateClient_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateClientCommand(
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var handler = new CreateClientCommandHandler(DbContext);

        // Act
        Result<Guid> result =  await handler.Handle(command, default);        

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_AddClientToDatabase_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateClientCommand(
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        var handler = new CreateClientCommandHandler(DbContext);

        // Act
        Result<Guid> result = await handler.Handle(command, default);

        // Assert
        IndividualClient? individualClient = await DbContext.IndividualClients.FindAsync(result.Value);

        individualClient.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateClientCommand(
            string.Empty,
            string.Empty);

        // Act
        TestValidationResult<CreateClientCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrors();
    }
}
