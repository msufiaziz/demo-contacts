using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories
{
	public interface IRepositoryAsync<T, in TId> where T : class, IEntity<TId>
	{
		IQueryable<T> Entities { get; }

		Task<T?> GetByIdAsync(TId id);

		Task<List<T>> GetAllAsync();

		Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

		Task<T> AddAsync(T entity);

		Task UpdateAsync(T entity);

		Task DeleteAsync(T entity);

		Task<int> CountAsync();
	}
}
