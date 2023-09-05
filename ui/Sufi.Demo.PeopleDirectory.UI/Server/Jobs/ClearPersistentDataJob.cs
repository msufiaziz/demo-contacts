using Quartz;
using Sufi.Demo.PeopleDirectory.Libs.DataContext;
using Sufi.Demo.PeopleDirectory.Libs.DataContext.Entities;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Jobs
{
	public class ClearPersistentDataJob : IJob
	{
		private const string LastDateDeletedKey = "LastDateDeleted";
		private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.ffff";
		private readonly AppDbContext _dbContext;

		public ClearPersistentDataJob(AppDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var contactsToDelete = _dbContext.Contacts.ToList();
			_dbContext.Contacts.RemoveRange(contactsToDelete);

			var infoToUpdate = await _dbContext.ServerInfos.FindAsync(LastDateDeletedKey);
			if (infoToUpdate != null)
			{
				infoToUpdate.Value = DateTime.UtcNow.ToString(DateTimeFormat);
			}
			else
			{
				var infoToAdd = new ServerInfo { Key = LastDateDeletedKey, Value = DateTime.UtcNow.ToString(DateTimeFormat) };
				await _dbContext.ServerInfos.AddAsync(infoToAdd);
			}

			await _dbContext.SaveChangesAsync();
        }
	}
}
