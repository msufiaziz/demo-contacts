﻿using Microsoft.EntityFrameworkCore;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using Sufi.Demo.PeropleDirectory.Infrastructure.Contexts;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Repositories
{
	public class RepositoryAsync<T, TId> : IRepositoryAsync<T, TId> where T : AuditableEntity<TId>
	{
		private readonly ApplicationDbContext _dbContext;

		public RepositoryAsync(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IQueryable<T> Entities => _dbContext.Set<T>();

		public async Task<T> AddAsync(T entity)
		{
			await _dbContext.Set<T>().AddAsync(entity);
			return entity;
		}

		public async Task<int> CountAsync() => await _dbContext.Set<T>().CountAsync();

		public Task DeleteAsync(T entity)
		{
			_dbContext.Set<T>().Remove(entity);
			return Task.CompletedTask;
		}

		public async Task<List<T>> GetAllAsync()
		{
			return await _dbContext
				.Set<T>()
				.ToListAsync();
		}

		public async Task<T?> GetByIdAsync(TId id)
		{
			return await _dbContext.Set<T>().FindAsync(id);
		}

		public async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
		{
			return await _dbContext
				.Set<T>()
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();
		}

		public Task UpdateAsync(T entity)
		{
			T? exist = _dbContext.Set<T>().Find(entity.Id);
			if (exist != null)
			{
				_dbContext.Entry<T>(exist).CurrentValues.SetValues(entity);
			}

			return Task.CompletedTask;
		}
	}
}
