using Sufi.Demo.PeopleDirectory.Domain.Common;

namespace Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories
{
	public interface IAsyncRepository<T, in TId> where T : class, IEntity<TId>
	{
		IQueryable<T> Entities { get; }

		Task<T?> GetByIdAsync(TId id);

		Task<List<T>> GetAllAsync();

		Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

		Task<T> AddAsync(T entity);

		Task UpdateAsync(T entity);

		Task DeleteAsync(T entity);
		Task<int> DeleteByIdAsync(TId id);

		Task<int> CountAsync();
	}
}
