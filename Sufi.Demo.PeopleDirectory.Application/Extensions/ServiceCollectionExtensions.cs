using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Sufi.Demo.PeopleDirectory.Application.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
		{
			services.AddAutoMapper(config => config.AddMaps(Assembly.GetExecutingAssembly()));
			services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
			services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

			return services;
		}
	}
}
