using System;
using System.Collections.Generic;
using System.Text;
using Application.Carts;
using Bogus;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Abstractions;

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest : IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly Faker Faker = new();    
    protected readonly CartService CartService;
    protected readonly ApplicationDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        CartService = _scope.ServiceProvider.GetRequiredService<CartService>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }
    public void Dispose()
    {
        _scope.Dispose();
    }
}
