using Serilog;
using Sufi.Demo.PeopleDirectory.UI.Server;

try
{
	var builder = WebApplication.CreateBuilder(args);
	var app = builder.ConfigureServices()
		.ConfigurePipeline();

	// Ensure all pending migrations are applied.
	await app.EnsureDatabaseMigrationAsync();

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
