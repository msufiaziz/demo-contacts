using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Repositories
{
	public class ContactRepository : IContactRepository
	{
		private readonly IRepositoryAsync<Contact, int> _repository;

		public ContactRepository(IRepositoryAsync<Contact, int> repository)
		{
			_repository = repository;
		}
	}
}
