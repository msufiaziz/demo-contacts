namespace Sufi.Demo.PeopleDirectory.UI.Server.Options
{
	/// <summary>
	/// Represents configuration options for rate limiting functionality.
	/// </summary>
	/// <remarks>This type is used to define the parameters for controlling the rate at which operations are
	/// allowed. It specifies the maximum number of permits and the time window during which the permits are
	/// valid.</remarks>
	public record RateLimitOptions
	{
		/// <summary>
		/// Gets the maximum number of permits that can be issued.
		/// </summary>
		public int PermitLimit { get; init; } = 100;
		/// <summary>
		/// Gets the time interval that defines the window for rate-limiting operations (in seconds).
		/// </summary>
		public int Window { get; init; } = 60;
	}
}
