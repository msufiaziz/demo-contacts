using Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Common;
using Sufi.Demo.PeopleDirectory.Persistence.Contexts;
using System.Collections;

namespace Sufi.Demo.PeopleDirectory.Persistence.Repositories
{
	public class UnitOfWork<TId>(
		ApplicationDbContext dbContext
		) : IUnitOfWork<TId>
	{
		private bool disposed;
		private Hashtable? _repositories;

		public IAsyncRepository<TEntity, TId> Repository<TEntity>() where TEntity : AuditableEntity<TId>
		{
			_repositories ??= [];

			var type = typeof(TEntity).Name;

			if (!_repositories.ContainsKey(type))
			{
				var repositoryType = typeof(AsyncRepository<,>);

				var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), dbContext);

				_repositories.Add(type, repositoryInstance);
			}

			return (IAsyncRepository<TEntity, TId>)_repositories[type]!;
		}

		public async Task<int> Commit(CancellationToken cancellationToken)
		{
			return await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
		{
			var result = await dbContext.SaveChangesAsync(cancellationToken);

			return result;
		}

		public Task Rollback()
		{
			dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					//dispose managed resources
					dbContext.Dispose();
				}
			}
			//dispose unmanaged resources
			disposed = true;
		}
	}
}
