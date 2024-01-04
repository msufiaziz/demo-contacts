using Quartz;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeropleDirectory.Infrastructure.Contexts;
using Sufi.Demo.PeropleDirectory.Infrastructure.Repositories;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Jobs
{
	/// <summary>
	/// Represents the cleanup job.
	/// </summary>
	public class ClearPersistentDataJob : IJob
	{
		private const string LastDateDeletedKey = "LastDateDeleted";
		private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.ffff";
		private readonly IUnitOfWork<int> _unitOfWorkInt;
		private readonly IUnitOfWork<string> _unitOfWorkString;

		/// <summary>
		/// Initialize an instance of <see cref="ClearPersistentDataJob"/> class.
		/// </summary>
		/// <param name="unitOfWorkInt"></param>
		/// <param name="unitOfWorkString"></param>
		public ClearPersistentDataJob(IUnitOfWork<int> unitOfWorkInt, IUnitOfWork<string> unitOfWorkString)
		{
			_unitOfWorkInt = unitOfWorkInt;
			_unitOfWorkString = unitOfWorkString;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var contactsToDelete = await _unitOfWorkInt.Repository<Contact>().GetAllAsync();
			foreach (var contact in contactsToDelete)
			{
				await _unitOfWorkInt.Repository<Contact>().DeleteAsync(contact);
			}
			await _unitOfWorkInt.Commit(context.CancellationToken);

			var infoToUpdate = await _unitOfWorkString.Repository<ServerInfo>().GetByIdAsync(LastDateDeletedKey);
			if (infoToUpdate != null)
			{
				infoToUpdate.Value = DateTime.UtcNow.ToString(DateTimeFormat);
				await _unitOfWorkString.Repository<ServerInfo>().UpdateAsync(infoToUpdate);
			}
			else
			{
				var infoToAdd = new ServerInfo { Id = LastDateDeletedKey, Value = DateTime.UtcNow.ToString(DateTimeFormat) };
				await _unitOfWorkString.Repository<ServerInfo>().AddAsync(infoToAdd);
			}

			await _unitOfWorkString.Commit(context.CancellationToken);
		}
	}
}
