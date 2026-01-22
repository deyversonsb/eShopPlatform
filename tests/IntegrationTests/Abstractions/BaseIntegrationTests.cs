using System;
using System.Collections.Generic;
using System.Text;
using Application.Carts;
using Bogus;
using Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Abstractions;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected Faker Faker { get; }  
    protected CartService CartService { get; }

    protected ApplicationDbContext DbContext { get; }

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        CartService = _scope.ServiceProvider.GetRequiredService<CartService>();
        DbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Faker = new();
    }
    public void Dispose()
    {
        _scope.Dispose();
    }
}
