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
    private readonly Mock<IApplicationDbContext> _dbContextMock;
    private readonly CreateClientCommandValidator validator;
    public CreateClientTests(IntegrationTestWebAppFactory factory)
        :base(factory)
    {
        _dbContextMock = new();
        validator = new CreateClientCommandValidator();
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
    [Fact]
    public async Task Should_CreateClient_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateClientCommand(
            Faker.Name.FirstName(),
            Faker.Name.LastName());

        _dbContextMock
            .Setup(db => db.IndividualClients.Add(IndividualClient.Create(
                Faker.Name.FirstName(),
                Faker.Name.LastName())));

        var handler = new CreateClientCommandHandler(_dbContextMock.Object);

        // Act
        TestValidationResult<CreateClientCommand> testValidationResult = await validator.TestValidateAsync(command);

        Result<Guid> result =  await handler.Handle(command, default);        

        // Assert
        testValidationResult.ShouldNotHaveAnyValidationErrors();

        result.IsSuccess.Should().BeTrue();
    }
}
