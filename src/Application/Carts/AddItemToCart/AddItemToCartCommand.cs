using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Carts.AddItemToCart;

public record AddItemToCartCommand(
	Guid ClientId,
	Guid ProductId,
	decimal Quantity,
	bool IsProfessionalClient) : ICommand;
