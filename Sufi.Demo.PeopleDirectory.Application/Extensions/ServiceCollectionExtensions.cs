using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Sufi.Demo.PeopleDirectory.Application.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddApplicationLayer(this IServiceCollection services, Type assemblyType)
		{
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
		}
	}
}
