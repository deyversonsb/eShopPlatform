using System;
using System.Collections.Generic;
using System.Text;
using Domain.Clients;
using Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
	DbSet<IndividualClient> IndividualClients { get; }
	DbSet<ProfessionalClient> ProfessionalClients { get; }
	DbSet<Product> Products { get; }
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
