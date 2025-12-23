using System;
using System.Collections.Generic;
using System.Text;
using SharedKernel;

namespace Domain.Orders;

public sealed class OrderItem : Entity
{
    public OrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        decimal quantity,
        decimal unitPrice) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Price = quantity * unitPrice;
	}

    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Price { get; private set; }

    public static OrderItem Create(
        Guid orderId,
        Guid productId,
        decimal quantity,
        decimal unitPrice)
    => new(Guid.NewGuid(), orderId, productId, quantity, unitPrice);
}
