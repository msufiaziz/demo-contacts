using AutoMapper;
using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetAll
{
	public class GetAllContactsQuery : IRequest<IResult<List<GetAllContactsResponse>>>
	{
	}

	public class GetAllContactsQueryHandler(
		IUnitOfWork<int> unitOfWork, 
		IMapper mapper,
		IAppCache appCache
		) : IRequestHandler<GetAllContactsQuery, IResult<List<GetAllContactsResponse>>>
	{
		public async Task<IResult<List<GetAllContactsResponse>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
		{
			Task<List<Contact>> allContactsFunc() => unitOfWork.Repository<Contact>().GetAllAsync();

			var allContacts = await appCache.GetOrAddAsync(
				"contact_all",
				async token => await allContactsFunc(),
				absoluteExpireTime: TimeSpan.FromMinutes(2),
				tags: ["contacts"]
			);

			var mappedContacts = mapper.Map<List<GetAllContactsResponse>>(allContacts);
			return await Result<List<GetAllContactsResponse>>.SuccessAsync(mappedContacts);
		}
	}
}
