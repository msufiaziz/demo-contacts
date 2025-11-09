using AutoMapper;
using FluentValidation;
using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetById
{
	public class GetContactByIdQuery : IRequest<IResult<GetContactByIdResponse>>
	{
		public int Id { get; set; }
	}

	public sealed class GetContactByIdQueryValidator : AbstractValidator<GetContactByIdQuery>
	{
		public GetContactByIdQueryValidator()
		{
			RuleFor(v => v.Id)
				.GreaterThan(0)
				.WithMessage("A valid Id is required.");
		}
	}

	public class GetContactByIdQueryHandler(
		IUnitOfWork<int> unitOfWork, 
		IMapper mapper,
		IAppCache appCache
		) : IRequestHandler<GetContactByIdQuery, IResult<GetContactByIdResponse>>
	{
		public async Task<IResult<GetContactByIdResponse>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
		{
			Task<Contact?> getContactByIdFunc() => unitOfWork.Repository<Contact>().GetByIdAsync(request.Id);

			var contact = await appCache.GetOrAddAsync(
				$"contact_{request.Id}",
				async token => await getContactByIdFunc(),
				absoluteExpireTime: TimeSpan.FromMinutes(2),
				tags: ["contacts"]
			);

			var mappedContact = mapper.Map<GetContactByIdResponse>(contact);
			return await Result<GetContactByIdResponse>.SuccessAsync(mappedContact);
		}
	}
}
