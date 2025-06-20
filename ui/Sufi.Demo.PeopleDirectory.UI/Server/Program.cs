using OpenTelemetry.Metrics;
using Quartz;
using Serilog;
using Sufi.Demo.PeopleDirectory.Application.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Jobs;
using Sufi.Demo.PeopleDirectory.UI.Server.Options;
using Sufi.Demo.PeropleDirectory.Infrastructure.Extensions;
using System.Threading.RateLimiting;

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
		.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
		.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true);
	var configuration = builder.Configuration;
	var rateLimitOptions = configuration.GetSection("RateLimit").Get<RateLimitOptions>()!;

	Log.Logger = new LoggerConfiguration()
		.ReadFrom.Configuration(configuration)
		.CreateLogger();

	builder.Host.UseSerilog((context, services, configuration) =>
	{
		configuration.ReadFrom.Configuration(context.Configuration);
	});
	var services = builder.Services;

	// Add services to the container.
	services.AddAutoMapper(typeof(Program));
	services.AddCurrentUserService();

	services.AddApplicationLayer(typeof(Program));
	services.AddApplicationServices();
	services.AddRepositories();
	services.AddInfrastructureMappings();

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

	services.AddDatabase(configuration);

	services.AddIdentity();

	// Register Swagger services.
	services.AddEndpointsApiExplorer();
	services.RegisterSwagger();

	// Some background jobs here.
	services.AddQuartz(options =>
	{
		var jobKey = new JobKey("ClearPersistentDataJob");
		options.AddJob<ClearPersistentDataJob>(opt => opt.WithIdentity(jobKey));
		options.AddTrigger(opt =>
		{
			opt.ForJob(jobKey)
			   .WithIdentity("ClearPersistentDataJob-trigger")
			   .WithSimpleSchedule(x => x
					.WithIntervalInMinutes(5)
					.RepeatForever());
		});
	});
	services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

	var app = builder.Build();

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

	// Ensure all pending migrations are applied.
	app.EnsureDatabaseMigration();

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

	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}
