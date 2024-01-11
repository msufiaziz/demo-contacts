using Asp.Versioning.ApiExplorer;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Services;
using Sufi.Demo.PeopleDirectory.UI.Server.Services;
using Sufi.Demo.PeropleDirectory.Infrastructure.Contexts;
using Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

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
			services.ConfigureOptions<ConfigureSwaggerGenOptions>();
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

				c.OperationFilter<SwaggerDefaultValues>();
			});
		}
	}

	public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider _provider;

		public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
		{
			_provider = provider;
		}

		public void Configure(SwaggerGenOptions options)
		{
			foreach (var description in _provider.ApiVersionDescriptions)
			{
				options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
			}
		}

		private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
		{
			var text = new StringBuilder("This application is for demo purposes by Sufi only.");
			var info = new OpenApiInfo
			{
				Title = "Sufi.Demo.App",
				Version = description.ApiVersion.ToString(),
				License = new OpenApiLicense
				{
					Name = "MIT License",
					Url = new Uri("https://opensource.org/licenses/MIT")
				}
			};

			if (description.IsDeprecated)
			{
				text.Append("This Api version has been deprecated.");
			}

			if (description.SunsetPolicy is SunsetPolicy policy && policy.Date is DateTimeOffset when)
			{
				text.Append(" The Api will be sunset on ")
					.Append(when.Date.ToShortDateString())
					.Append('.');
			}

			info.Description = text.ToString();

			return info;
		}
	}

	public class SwaggerDefaultValues : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var apiDescription = context.ApiDescription;

			operation.Deprecated |= apiDescription.IsDeprecated();

			foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
			{
				var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
				var response = operation.Responses[responseKey];

				foreach (var contentType in response.Content.Keys)
				{
					if (!responseType.ApiResponseFormats.Any(x => x.MediaType == contentType))
					{
						response.Content.Remove(contentType);
					}
				}
			}

			if (operation.Parameters == null)
			{
				return;
			}

            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

				parameter.Description ??= description.ModelMetadata.Description;

				if (parameter.Schema.Default == null && 
					description.DefaultValue != null && 
					description.DefaultValue is not DBNull && 
					description.ModelMetadata is ModelMetadata modelMetadata)
				{
					var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
					parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
				}

				parameter.Required |= description.IsRequired;
            }
        }
	}
}
