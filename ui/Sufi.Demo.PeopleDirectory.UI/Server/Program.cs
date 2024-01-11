using Quartz;
using Sufi.Demo.PeopleDirectory.Application.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Extensions;
using Sufi.Demo.PeopleDirectory.UI.Server.Jobs;
using Sufi.Demo.PeropleDirectory.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);
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
		   .WithDailyTimeIntervalSchedule(x => x.WithIntervalInMinutes(5));
	});
});
services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var app = builder.Build();

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

app.MapHealthChecks("/health");
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
