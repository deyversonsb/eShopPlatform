using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Caching;

namespace Application.Carts;

public sealed class CartService(ICacheService cacheService)
{
	private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(20);
	public async Task<Cart> GetAsync(Guid clientId, CancellationToken cancellationToken = default)
	{
		string cacheKey = CreateCacheKey(clientId);

		Cart cart = await cacheService.GetAsync<Cart>(cacheKey, cancellationToken) ??
					Cart.CreateDefault(clientId);

		cart.SetTotalPrice();

		return cart;
	}
	public async Task ClearAsync(Guid clientId, CancellationToken cancellationToken = default)
	{
		string cacheKey = CreateCacheKey(clientId);

		var cart = Cart.CreateDefault(clientId);

		await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
	}
	public async Task AddItemAsync(Guid clientId, CartItem cartItem, CancellationToken cancellationToken = default)
	{
		string cacheKey = CreateCacheKey(clientId);

		Cart cart = await GetAsync(clientId, cancellationToken);

		CartItem? existingCartItem = cart.Items.Find(c => c.ProductId == cartItem.ProductId);

        if (existingCartItem is null)
        {
            cart.Items.Add(cartItem);
        }
        else
        {
            existingCartItem.Quantity = cartItem.Quantity;
        }

		await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
	}
	public async Task RemoveItemAsync(Guid clientId, Guid ticketTypeId, CancellationToken cancellationToken = default)
	{
		string cacheKey = CreateCacheKey(clientId);

		Cart cart = await GetAsync(clientId, cancellationToken);

		CartItem? cartItem = cart.Items.Find(c => c.ProductId == ticketTypeId);

		if (cartItem is null)
		{
			return;
		}

		cart.Items.Remove(cartItem);

		await cacheService.SetAsync(cacheKey, cart, DefaultExpiration, cancellationToken);
	}
	private static string CreateCacheKey(Guid clientId) => $"carts:{clientId}";
}
