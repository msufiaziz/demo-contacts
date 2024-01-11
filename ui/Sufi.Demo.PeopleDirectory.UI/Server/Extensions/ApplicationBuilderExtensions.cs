using Microsoft.EntityFrameworkCore;
using Sufi.Demo.PeropleDirectory.Infrastructure.Contexts;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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

		internal static IApplicationBuilder EnsureDatabaseMigration(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				dbContext.Database.Migrate();
			}

			return app;
		}
	}
}
