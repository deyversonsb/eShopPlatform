using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;

namespace Infrastructure.Caching;

internal static class CacheOptions
{
	internal static DistributedCacheEntryOptions DefaultExpiration => new()
	{
		AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
	};

	internal static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
		expiration is not null ? 
			new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration }
			: DefaultExpiration;		
}
