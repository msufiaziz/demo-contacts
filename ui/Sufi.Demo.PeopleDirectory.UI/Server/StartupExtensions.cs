using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Serilog;
using Sufi.Demo.PeopleDirectory.Application.Extensions;
using Sufi.Demo.PeopleDirectory.Infrastructure;
using Sufi.Demo.PeopleDirectory.Persistence.Contexts;
using Sufi.Demo.PeopleDirectory.Persistence.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Options;
using System.Threading.RateLimiting;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Sufi.Demo.PeopleDirectory.UI.Server
{
	public static class StartupExtensions
	{
		public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
		{
			var configuration = builder.Configuration;

			configuration.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true);

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(configuration)
				.CreateLogger();

			builder.Host.UseSerilog((context, services, configuration) =>
			{
				configuration.ReadFrom.Configuration(context.Configuration);
			});

			var rateLimitOptions = configuration.GetSection("RateLimit").Get<RateLimitOptions>()!;
			var services = builder.Services;

			// Add services to the container.
			services.AddHttpContextAccessor();
			services.AddApplicationLayer()
				.AddInfrastructureServices()
				.AddPersistenceServices(configuration);

			services.AddControllersWithViews();
			services.AddRazorPages();
			services.AddHealthChecks();

			services.AddRateLimiter(options =>
			{
				// Apply for all requests.
				options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
					RateLimitPartition.GetFixedWindowLimiter(
						partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
						factory: partition => new FixedWindowRateLimiterOptions
						{
							AutoReplenishment = true,
							PermitLimit = rateLimitOptions.PermitLimit,
							Window = TimeSpan.FromSeconds(rateLimitOptions.Window)
						}));

				options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
			});

			// Add metrics and tracing services.
			services.AddOpenTelemetry()
				.WithMetrics(builder =>
				{
					builder.AddPrometheusExporter()
						.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
				});

			// Register Swagger services.
			services.AddEndpointsApiExplorer();
			services.RegisterSwagger();

			return builder.Build();
		}

		public static WebApplication ConfigurePipeline(this WebApplication app)
		{
			app.UseSerilogRequestLogging();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseWebAssemblyDebugging();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			// Configure OpenTelemetry for metrics and tracing.
			app.UseOpenTelemetryPrometheusScrapingEndpoint();

			app.ConfigureSwagger();

			app.UseBlazorFrameworkFiles();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseRateLimiter();

			app.MapHealthChecks("/health");
			app.MapRazorPages();
			app.MapControllers();
			app.MapFallbackToFile("index.html");

			return app;
		}

		public static void ConfigureSwagger(this IApplicationBuilder app)
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

		public static async Task<IApplicationBuilder> EnsureDatabaseMigrationAsync(this IApplicationBuilder app)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				await dbContext.Database.MigrateAsync();
			}

			return app;
		}
	}
}
