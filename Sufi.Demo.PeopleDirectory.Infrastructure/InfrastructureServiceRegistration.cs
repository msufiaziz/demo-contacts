using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using Sufi.Demo.PeopleDirectory.Infrastructure.Identity;
using Sufi.Demo.PeopleDirectory.Infrastructure.Jobs;
using Sufi.Demo.PeopleDirectory.Infrastructure.Services;

namespace Sufi.Demo.PeopleDirectory.Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
		{
			services.AddHybridCache();

			services.AddTransient<ICurrentUserService, CurrentUserService>()
				.AddTransient<IAppCache, AppCache>();

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

			return services;
		}
	}
}
