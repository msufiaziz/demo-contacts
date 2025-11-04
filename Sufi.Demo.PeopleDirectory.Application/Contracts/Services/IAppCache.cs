namespace Sufi.Demo.PeopleDirectory.Application.Contracts.Services
{
	public interface IAppCache
	{
		ValueTask<T> GetOrAddAsync<T>(string key, Func<CancellationToken, ValueTask<T>> factory, 
			IEnumerable<string>? tags = null, TimeSpan? absoluteExpireTime = null);
		ValueTask RemoveAsync(string key);
		ValueTask RemoveByTagAsync(string tag);
		ValueTask ResetAsync();
	}
}
