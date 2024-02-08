using AutoMapper;
using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;
using System.Threading;
using System.Threading.Tasks;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetById
{
	public class GetContactByIdQuery : IRequest<Result<GetContactByIdResponse>>
	{
		public int Id { get; set; }
	}

	public class GetContactByIdQueryHandler : IRequestHandler<GetContactByIdQuery, Result<GetContactByIdResponse>>
	{
		private readonly IUnitOfWork<int> _unitOfWork;
		private readonly IMapper _mapper;

		public GetContactByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<Result<GetContactByIdResponse>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
		{
			var contact = await _unitOfWork.Repository<Contact>().GetByIdAsync(request.Id);
			var mappedContact = _mapper.Map<GetContactByIdResponse>(contact);
			return await Result<GetContactByIdResponse>.SuccessAsync(mappedContact);
		}
	}
}
