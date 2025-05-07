using AutoMapper;
using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetById
{
	public class GetContactByIdQuery : IRequest<IResult<GetContactByIdResponse>>
	{
		public int Id { get; set; }
	}

	public class GetContactByIdQueryHandler(
		IUnitOfWork<int> unitOfWork, 
		IMapper mapper
		) : IRequestHandler<GetContactByIdQuery, IResult<GetContactByIdResponse>>
	{
		public async Task<IResult<GetContactByIdResponse>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
		{
			var contact = await unitOfWork.Repository<Contact>().GetByIdAsync(request.Id);
			var mappedContact = mapper.Map<GetContactByIdResponse>(contact);
			return await Result<GetContactByIdResponse>.SuccessAsync(mappedContact);
		}
	}
}
