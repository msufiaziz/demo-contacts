using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Services;
using Sufi.Demo.PeopleDirectory.UI.Server.Services;
using Sufi.Demo.PeropleDirectory.Infrastructure.Contexts;
using Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Extensions
{
	public static class ServiceCollectionExtensions
	{
		internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			//services.AddTransient<IContactService, ContactService>();
			return services;
		}

		internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
		{
			services.AddHttpContextAccessor();
			services.AddScoped<ICurrentUserService, CurrentUserService>();
			return services;
		}

		internal static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
			=> services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionString")!));

		internal static IServiceCollection AddIdentity(this IServiceCollection services)
		{
			services
				.AddIdentity<AppUser, AppRole>(options =>
				{
					options.Password.RequiredLength = 6;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.User.RequireUniqueEmail = true;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			return services;
		}

		internal static void RegisterSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				//TODO - Lowercase Swagger Documents
				//c.DocumentFilter<LowercaseDocumentFilter>();
				//Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

				// include all project's xml comments
				var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (!assembly.IsDynamic)
					{
						var xmlFile = $"{assembly.GetName().Name}.xml";
						var xmlPath = Path.Combine(baseDirectory, xmlFile);
						if (File.Exists(xmlPath))
						{
							c.IncludeXmlComments(xmlPath);
						}
					}
				}

				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Sufi.Demo.App",
					License = new OpenApiLicense
					{
						Name = "MIT License",
						Url = new Uri("https://opensource.org/licenses/MIT")
					}
				});
			});
		}
	}
}
