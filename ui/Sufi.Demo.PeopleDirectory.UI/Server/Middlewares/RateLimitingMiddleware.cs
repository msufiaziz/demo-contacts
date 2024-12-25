using System.Collections.Concurrent;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Middlewares
{
	/// <summary>
	/// A middleware class for limiting API call rates.
	/// </summary>
	/// <remarks>
	/// Instantiate an instance of <see cref="RateLimitingMiddleware"/> class.
	/// </remarks>
	/// <param name="next"></param>
	/// <param name="maxRequests"></param>
	/// <param name="timeSpan"></param>
	public class RateLimitingMiddleware(RequestDelegate next, int maxRequests, TimeSpan timeSpan)
	{
		private static readonly SemaphoreSlim Semaphore = new(1, 1);
		private static readonly ConcurrentDictionary<string, (int count, DateTime resetTime)> RequestCounts = new();

		/// <summary>
		/// Method to be invoked for a HTTP request.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task InvokeAsync(HttpContext context)
		{
			var clientIdentifier = GetClientIdentifier(context);

			// Check if the request is allowed
			bool isAllowed;
			int retryAfterSeconds;

			await Semaphore.WaitAsync();
			try
			{
				isAllowed = CheckRateLimit(clientIdentifier, out retryAfterSeconds);
			}
			finally
			{
				Semaphore.Release();
			}

			// If the request is not allowed, return a 429 response
			if (!isAllowed)
			{
				context.Response.StatusCode = 429; // Too Many Requests
				context.Response.Headers.RetryAfter = retryAfterSeconds.ToString();
				await context.Response.WriteAsync($"Rate limit exceeded. Try again after {retryAfterSeconds} seconds.");
				return;
			}

			// Proceed to the next middleware
			await next(context);
		}

		private static string GetClientIdentifier(HttpContext context)
		{
			// Use the client's IP address as the identifier
			return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		}

		private bool CheckRateLimit(string clientIdentifier, out int retryAfterSeconds)
		{
			var now = DateTime.UtcNow;

			// Retrieve or initialize rate limit data for the client
			if (!RequestCounts.TryGetValue(clientIdentifier, out var entry) || entry.resetTime <= now)
			{
				// Reset rate limit state
				RequestCounts[clientIdentifier] = (1, now.Add(timeSpan));
				retryAfterSeconds = 0;
				return true;
			}

			var (count, resetTime) = entry;

			if (count >= maxRequests)
			{
				// Rate limit exceeded
				retryAfterSeconds = (int)(resetTime - now).TotalSeconds;
				return false;
			}

			// Increment request count
			RequestCounts[clientIdentifier] = (count + 1, resetTime);
			retryAfterSeconds = 0;
			return true;
		}
	}
}