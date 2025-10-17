using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using Sufi.Demo.PeopleDirectory.Infrastructure.Identity;
using Sufi.Demo.PeopleDirectory.Infrastructure.Jobs;

namespace Sufi.Demo.PeopleDirectory.Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
		{
			services.AddTransient<ICurrentUserService, CurrentUserService>();

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
