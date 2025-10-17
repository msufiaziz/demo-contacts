using Sufi.Demo.PeopleDirectory.Domain.Common;

namespace Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories
{
	public interface IUnitOfWork<TId> : IDisposable
	{
		IAsyncRepository<T, TId> Repository<T>() where T : AuditableEntity<TId>;

		Task<int> Commit(CancellationToken cancellationToken);

		Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

		Task Rollback();
	}
}
