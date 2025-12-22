using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Carts;
using Infrastructure.Caching;
using Infrastructure.Database;
using Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
	public static IServiceCollection AddInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	=> services
		.AddServices(configuration)
		.AddDatabase(configuration);

	private static IServiceCollection AddServices(
		this IServiceCollection services, 
		IConfiguration configuration)
	{
		services.AddSingleton<CartService>();
		services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

		string redisConnectionString = configuration.GetConnectionString("Cache")!;

		try
		{
			IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
			services.AddSingleton(connectionMultiplexer);

			services.AddStackExchangeRedisCache(options =>
				options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
		}
		catch
		{
			services.AddDistributedMemoryCache();
		}		

		services.TryAddSingleton<ICacheService, CacheService>();

		return services;
	}

	private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
        string? connectionString = configuration.GetConnectionString("Database");

		services.AddDbContext<ApplicationDbContext>(
			options => options
				.UseNpgsql(connectionString, npgsqlOptions =>
					npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
				.UseSnakeCaseNamingConvention());

		services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

		return services;
	}
}
