﻿namespace Sufi.Demo.PeopleDirectory.UI.Server.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		internal static void ConfigureSwagger(this IApplicationBuilder app)
		{
			app.UseSwagger();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
				options.RoutePrefix = "swagger";
				options.DisplayRequestDuration();
			});
		}
	}
}
