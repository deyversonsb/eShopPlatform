using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Carts;

public sealed class Cart
{
	public Guid ClientId { get; init; }

	public List<CartItem> Items { get; init; } = [];

	public decimal TotalPrice { get; private set; }

	internal static Cart CreateDefault(Guid clientId) => new() { ClientId = clientId };

	internal void SetTotalPrice()
	{
		TotalPrice = Items.Sum(i => i.Quantity * i.Price);
	}
}
