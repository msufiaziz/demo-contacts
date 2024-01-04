using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands
{
	public class DeleteContactCommand : IRequest<IResult>
	{
		[Required]
		public int Id { get; set; }
	}

	internal class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, IResult>
	{
		private readonly IUnitOfWork<int> _unitOfWork;

		public DeleteContactCommandHandler(IUnitOfWork<int> unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IResult> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
		{
			var itemToDelete = await _unitOfWork.Repository<Contact>().GetByIdAsync(request.Id);
            if (itemToDelete != null)
            {
				await _unitOfWork.Repository<Contact>().DeleteAsync(itemToDelete);
				await _unitOfWork.Commit(cancellationToken);
				return Result.Success();
			}

			return Result.Fail("No data to delete.");
            
		}
	}
}
