using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;
using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands
{
	public class DeleteContactCommand : IRequest<IResult>
	{
		[Required]
		public int Id { get; set; }
	}

	internal class DeleteContactCommandHandler(
		IUnitOfWork<int> unitOfWork
		) : IRequestHandler<DeleteContactCommand, IResult>
	{
		public async Task<IResult> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
		{
			var itemToDelete = await unitOfWork.Repository<Contact>().GetByIdAsync(request.Id);
            if (itemToDelete != null)
            {
				await unitOfWork.Repository<Contact>().DeleteAsync(itemToDelete);
				await unitOfWork.Commit(cancellationToken);
				return Result.Success();
			}

			return Result.Fail("No data to delete.");
		}
	}
}
