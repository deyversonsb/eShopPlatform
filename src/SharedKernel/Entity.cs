using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel;

public abstract class Entity
{
    public Guid Id { get; init; }

    protected Entity(Guid id)
    {
        Id = id;
    }
}