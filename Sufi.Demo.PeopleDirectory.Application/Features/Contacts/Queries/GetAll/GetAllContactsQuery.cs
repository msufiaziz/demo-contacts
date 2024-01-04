using AutoMapper;
using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetAll
{
	public class GetAllContactsQuery : IRequest<Result<List<GetAllContactsResponse>>>
	{
	}

	internal class GetAllContactsQueryHandler : IRequestHandler<GetAllContactsQuery, Result<List<GetAllContactsResponse>>>
	{
		private readonly IUnitOfWork<int> _unitOfWork;
		private readonly IMapper _mapper;

		public GetAllContactsQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<Result<List<GetAllContactsResponse>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
		{
			var allContacts = await _unitOfWork.Repository<Contact>().GetAllAsync();
			var mappedContacts = _mapper.Map<List<GetAllContactsResponse>>(allContacts);
			return await Result<List<GetAllContactsResponse>>.SuccessAsync(mappedContacts);
		}
	}
}
