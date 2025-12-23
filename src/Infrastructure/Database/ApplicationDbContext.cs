using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Data;
using Domain.Clients;
using Domain.Orders;
using Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<IndividualClient> IndividualClients { get; set; }
	public DbSet<ProfessionalClient> ProfessionalClients { get; set; }
	public DbSet<Product> Products { get; set; }
	public DbSet<Order> Orders { get; set; }
	public DbSet<OrderItem> OrderItems { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);
	}
	public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		int result = await base.SaveChangesAsync(cancellationToken);

		return result;
	}
}
