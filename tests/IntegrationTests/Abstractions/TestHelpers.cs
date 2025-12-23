using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedKernel;

namespace IntegrationTests.Abstractions;

internal static class TestHelpers
{
    internal static Mock<DbSet<TEntity>> GetMockDbSet<TEntity>(IQueryable<TEntity> entities)
        where TEntity : Entity
    {
        var mockDbSet = new Mock<DbSet<TEntity>>();
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.Provider).Returns(entities.Provider);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.Expression).Returns(entities.Expression);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.ElementType).Returns(entities.ElementType);
        mockDbSet.As<IQueryable<TEntity>>()
            .Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());

        return mockDbSet;
    }
}
