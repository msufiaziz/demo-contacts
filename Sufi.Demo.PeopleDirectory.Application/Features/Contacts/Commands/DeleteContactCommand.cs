using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands
{
	public class DeleteContactCommand : IRequest<IResult>
	{
		public int Id { get; set; }
	}

	public sealed class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
	{
		public DeleteContactCommandValidator()
		{
			RuleFor(v => v.Id)
				.GreaterThan(0)
				.WithMessage("A valid Id is required.");
		}
	}

	public class DeleteContactCommandHandler(
			IUnitOfWork<int> unitOfWork,
			ILogger<DeleteContactCommandHandler> logger,
			IAppCache appCache
			) : IRequestHandler<DeleteContactCommand, IResult>
	{
		public async Task<IResult> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
		{
			var itemToDelete = await unitOfWork.Repository<Contact>().GetByIdAsync(request.Id);
			if (itemToDelete != null)
			{
				await unitOfWork.Repository<Contact>().DeleteByIdAsync(request.Id);

				// Clear cache entries related to contacts.
				await appCache.RemoveAsync($"contact_{request.Id}");
				await appCache.RemoveAsync("contact_all");

				logger.LogInformation("Contact with ID: {Id} deleted.", request.Id);

				return Result.Success();
			}

			logger.LogWarning("No contact found with ID: {Id}", request.Id);

			return Result.Fail("No data to delete.");
		}
	}
}