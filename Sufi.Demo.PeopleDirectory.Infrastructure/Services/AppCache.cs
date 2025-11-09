using Microsoft.Extensions.Caching.Hybrid;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;

namespace Sufi.Demo.PeopleDirectory.Infrastructure.Services
{
	public class AppCache(
		HybridCache hybridCache
		) : IAppCache
	{
		public ValueTask<T> GetOrAddAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, 
			IEnumerable<string>? tags = null, TimeSpan? absoluteExpireTime = null)
		{
			var options = new HybridCacheEntryOptions
			{
				Expiration = absoluteExpireTime ?? TimeSpan.FromSeconds(30),
			};

			return hybridCache.GetOrCreateAsync<T>(key, factory, options, tags);
		}

		public ValueTask RemoveAsync(string key)
		{
			return hybridCache.RemoveAsync(key);
		}

		public ValueTask RemoveByTagAsync(string tag)
		{
			return hybridCache.RemoveByTagAsync(tag);
		}

		public ValueTask ResetAsync()
		{
			return hybridCache.RemoveByTagAsync("*");
		}
	}
}
