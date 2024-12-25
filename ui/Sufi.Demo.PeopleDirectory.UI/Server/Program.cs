using Quartz;
using Serilog;
using Serilog.Events;
using Sufi.Demo.PeopleDirectory.Application.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Jobs;
using Sufi.Demo.PeopleDirectory.UI.Server.Middlewares;
using Sufi.Demo.PeropleDirectory.Infrastructure.Extensions;

Log.Logger = new LoggerConfiguration()
	//.MinimumLevel.Debug()
	.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
	.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
	.MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
	.Enrich.FromLogContext()
	.WriteTo.File("C:\\Logs\\demo-contact\\log-.log", rollingInterval: RollingInterval.Day)
	.CreateLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Host.UseSerilog();
	var services = builder.Services;
	var configuration = builder.Configuration;

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

	app.ConfigureSwagger();

	app.UseBlazorFrameworkFiles();
	app.UseStaticFiles();

	app.UseRouting();
	app.UseMiddleware<RateLimitingMiddleware>(100, TimeSpan.FromMinutes(1));	// Limit to 100 requests per minute per IP.

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
