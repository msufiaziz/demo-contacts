using Microsoft.EntityFrameworkCore;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Common;
using Sufi.Demo.PeopleDirectory.Persistence.Contexts;

namespace Sufi.Demo.PeopleDirectory.Persistence.Repositories
{
	public class AsyncRepository<T, TId>(
		ApplicationDbContext dbContext
		) : IAsyncRepository<T, TId> where T : AuditableEntity<TId>
	{
		public IQueryable<T> Entities => dbContext.Set<T>();

		public async Task<T> AddAsync(T entity)
		{
			await dbContext.Set<T>().AddAsync(entity);
			return entity;
		}

		public async Task<int> CountAsync() => await dbContext.Set<T>().CountAsync();

		public Task DeleteAsync(T entity)
		{
			dbContext.Set<T>().Remove(entity);
			return Task.CompletedTask;
		}

		public async Task<int> DeleteByIdAsync(TId id)
		{
			var rowsAffected = await dbContext.Set<T>()
				.Where(e => e.Id != null && e.Id.Equals(id))
				.ExecuteDeleteAsync();

			return rowsAffected;
		}

		public async Task<List<T>> GetAllAsync()
		{
			return await dbContext
				.Set<T>()
				.ToListAsync();
		}

		public async Task<T?> GetByIdAsync(TId id)
		{
			return await dbContext.Set<T>().FindAsync(id);
		}

		public async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
		{
			return await dbContext
				.Set<T>()
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public Task UpdateAsync(T entity)
		{
			T? exist = dbContext.Set<T>().Find(entity.Id);
			if (exist != null)
			{
				dbContext.Entry<T>(exist).CurrentValues.SetValues(entity);
			}

			return Task.CompletedTask;
		}
	}
}
