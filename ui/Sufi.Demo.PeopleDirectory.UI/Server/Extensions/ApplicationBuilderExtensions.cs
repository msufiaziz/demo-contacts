namespace Sufi.Demo.PeopleDirectory.UI.Server.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		internal static void ConfigureSwagger(this IApplicationBuilder app)
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.RoutePrefix = "swagger";
				options.DisplayRequestDuration();

				foreach (var desc in ((IEndpointRouteBuilder)app).DescribeApiVersions())
				{
					options.SwaggerEndpoint($"{desc.GroupName}/swagger.json", 
						desc.GroupName.ToUpperInvariant());
				}
			});
		}
	}
}
