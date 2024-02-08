using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Services;
using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using Sufi.Demo.PeropleDirectory.Infrastructure.Contexts;
using System.Collections;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Repositories
{
	public class UnitOfWork<TId> : IUnitOfWork<TId>
	{
		private readonly ICurrentUserService _currentUserService;
		private readonly ApplicationDbContext _dbContext;
		private bool disposed;
		private Hashtable? _repositories;

		public UnitOfWork(ApplicationDbContext dbContext, ICurrentUserService currentUserService)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			_currentUserService = currentUserService;
		}

		public IRepositoryAsync<TEntity, TId> Repository<TEntity>() where TEntity : AuditableEntity<TId>
		{
			_repositories ??= [];

			var type = typeof(TEntity).Name;

			if (!_repositories.ContainsKey(type))
			{
				var repositoryType = typeof(RepositoryAsync<,>);

				var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), _dbContext);

				_repositories.Add(type, repositoryInstance);
			}

			return (IRepositoryAsync<TEntity, TId>)_repositories[type]!;
		}

		public async Task<int> Commit(CancellationToken cancellationToken)
		{
			return await _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
		{
			var result = await _dbContext.SaveChangesAsync(cancellationToken);

			return result;
		}

		public Task Rollback()
		{
			_dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
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
					_dbContext.Dispose();
				}
			}
			//dispose unmanaged resources
			disposed = true;
		}
	}
}
