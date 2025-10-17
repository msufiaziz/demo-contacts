using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories;
using Sufi.Demo.PeopleDirectory.Persistence.Contexts;
using Sufi.Demo.PeopleDirectory.Persistence.Models.Identity;
using Sufi.Demo.PeopleDirectory.Persistence.Repositories;

namespace Sufi.Demo.PeopleDirectory.Persistence.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString")!));

			services
				.AddIdentityCore<AppUser>(options =>
				{
					options.Password.RequiredLength = 6;
					options.Password.RequireDigit = false;
					options.Password.RequireLowercase = false;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.User.RequireUniqueEmail = true;
				})
				.AddRoles<AppRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services
				.AddTransient(typeof(IAsyncRepository<,>), typeof(AsyncRepository<,>))
				.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

			return services;
		}
	}
}
