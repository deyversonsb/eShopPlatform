using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Carts;

public sealed class CartItem
{
	public Guid ProductId { get; set; }

	public decimal Quantity { get; set; }

	public decimal Price { get; set; }
}