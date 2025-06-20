using MediatR;
using Microsoft.Extensions.Logging;
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

	public class DeleteContactCommandHandler(
		IUnitOfWork<int> unitOfWork,
		ILogger<DeleteContactCommandHandler> logger
		) : IRequestHandler<DeleteContactCommand, IResult>
	{
		public async Task<IResult> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
		{
			var itemToDelete = await unitOfWork.Repository<Contact>().GetByIdAsync(request.Id);
            if (itemToDelete != null)
            {
				await unitOfWork.Repository<Contact>().DeleteAsync(itemToDelete);
				await unitOfWork.Commit(cancellationToken);

				logger.LogInformation("Contact with ID: {Id} deleted.", request.Id);

				return Result.Success();
			}

			logger.LogWarning("No contact found with ID: {Id}", request.Id);

			return Result.Fail("No data to delete.");
		}
	}
}
