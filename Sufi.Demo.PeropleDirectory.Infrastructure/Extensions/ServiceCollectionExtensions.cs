using Microsoft.Extensions.DependencyInjection;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeropleDirectory.Infrastructure.Repositories;
using System.Reflection;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddInfrastructureMappings(this IServiceCollection services)
		{
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
		}

		public static IServiceCollection AddRepositories(this IServiceCollection services)
		{
			return services
				.AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>))
				.AddTransient<IContactRepository, ContactRepository>()
				.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
		}
	}
}
