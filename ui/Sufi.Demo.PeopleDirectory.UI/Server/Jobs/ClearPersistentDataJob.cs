using Quartz;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Jobs
{
	/// <summary>
	/// Represents the cleanup job.
	/// </summary>
	/// <remarks>
	/// Initialize an instance of <see cref="ClearPersistentDataJob"/> class.
	/// </remarks>
	/// <param name="unitOfWorkInt"></param>
	/// <param name="unitOfWorkString"></param>
	public class ClearPersistentDataJob(IUnitOfWork<int> unitOfWorkInt, IUnitOfWork<string> unitOfWorkString) : IJob
	{
		private const string LastDateDeletedKey = "LastDateDeleted";
		private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.ffff";

		/// <summary>
		/// Job implementation to be executed.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task Execute(IJobExecutionContext context)
		{
			// Get all contacts to be deleted.
			var contactsToDelete = await unitOfWorkInt.Repository<Contact>().GetAllAsync();

			// Skips if nothing to delete.
			if (contactsToDelete.Count == 0)
				return;

			// Delete all contacts.
			foreach (var contact in contactsToDelete)
			{
				await unitOfWorkInt.Repository<Contact>().DeleteAsync(contact);
			}

			// Commit the changes to the database.
			await unitOfWorkInt.Commit(context.CancellationToken);

			// Update the last date deleted in the ServerInfo table.
			var infoToUpdate = await unitOfWorkString.Repository<ServerInfo>().GetByIdAsync(LastDateDeletedKey);
			if (infoToUpdate != null)
			{
				infoToUpdate.Value = DateTime.UtcNow.ToString(DateTimeFormat);
				await unitOfWorkString.Repository<ServerInfo>().UpdateAsync(infoToUpdate);
			}
			else
			{
				var infoToAdd = new ServerInfo { Id = LastDateDeletedKey, Value = DateTime.UtcNow.ToString(DateTimeFormat) };
				await unitOfWorkString.Repository<ServerInfo>().AddAsync(infoToAdd);
			}

			// Commit the changes to the database.
			await unitOfWorkString.Commit(context.CancellationToken);
		}
	}
}
