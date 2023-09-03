using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sufi.Demo.PeopleDirectory.Libs.DataContext;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
services.AddAutoMapper(typeof(Program));
services.AddControllersWithViews();
services.AddRazorPages();

var connectionString = configuration.GetConnectionString("MariaDbConnectionString");
services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString!));

// Register Swagger services.
services.AddEndpointsApiExplorer()
	.AddSwaggerGen(options =>
	{
		options.SwaggerDoc("v1", new OpenApiInfo { Title = "sufiaziz.my Demo API", Version = "v1" });
	});

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

	// Ensure all pending migrations are applied.
	using var scope = app.Services.CreateScope();
	var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
	options.SwaggerEndpoint("/swagger/v1/swagger.json", "sufiaziz.my API v1");
});

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
