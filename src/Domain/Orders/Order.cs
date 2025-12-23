using System;
using System.Collections.Generic;
using System.Text;
using Domain.Products;
using SharedKernel;

namespace Domain.Orders;

public sealed class Order : Entity
{
	private readonly List<OrderItem> _orderItems = [];

	private Order(
		Guid id,
		Guid clientId,
		DateTime createdAtUtc) : base(id)
	{ 
		ClientId = clientId;
		CreatedAtUtc = createdAtUtc;
	}

    public Guid ClientId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.ToList();
	public static Order Create(Guid clientId, DateTime createadAtUtc) 
	=> new(Guid.NewGuid(), clientId, createadAtUtc);

	public void AddItem(Product product, decimal quantity, decimal price)
	{
		var orderItem = OrderItem.Create(Id, product.Id, quantity, price);

		_orderItems.Add(orderItem);

		TotalPrice = _orderItems.Sum(oi => oi.Price);
	}
}
