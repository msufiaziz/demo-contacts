using AutoMapper;
using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetAll
{
	public class GetAllContactsQuery : IRequest<IResult<List<GetAllContactsResponse>>>
	{
	}

	internal class GetAllContactsQueryHandler(
		IUnitOfWork<int> unitOfWork, 
		IMapper mapper
		) : IRequestHandler<GetAllContactsQuery, IResult<List<GetAllContactsResponse>>>
	{
		public async Task<IResult<List<GetAllContactsResponse>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
		{
			var allContacts = await unitOfWork.Repository<Contact>().GetAllAsync();
			var mappedContacts = mapper.Map<List<GetAllContactsResponse>>(allContacts);
			return await Result<List<GetAllContactsResponse>>.SuccessAsync(mappedContacts);
		}
	}
}
