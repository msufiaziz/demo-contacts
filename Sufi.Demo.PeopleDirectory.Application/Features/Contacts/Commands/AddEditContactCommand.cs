using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Repositories;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands
{
	public class AddEditContactCommand : IRequest<IResult<int>>
	{
		public int Id { get; set; }
		public string UserName { get; set; } = "username";
		public string Phone { get; set; } = Random.Shared.Next(1000000000, 1999999999).ToString();
		public string Email { get; set; } = "user@example.com";
		public string SkillSets { get; set; } = "skill1, skill2, skill3";
		public string Hobby { get; set; } = "Hobby";
	}

	public sealed class AddEditContactCommandValidator : AbstractValidator<AddEditContactCommand>
	{
		public AddEditContactCommandValidator()
		{
			RuleFor(v => v.UserName)
				.NotEmpty()
				.WithMessage("UserName is required.")
				.MaximumLength(50)
				.WithMessage("UserName must not exceed 50 characters.");
			RuleFor(v => v.Phone)
				.NotEmpty()
				.WithMessage("Phone is required.")
				.MaximumLength(20)
				.WithMessage("Phone must not exceed 20 characters.");
			RuleFor(v => v.Email)
				.NotEmpty()
				.WithMessage("Email is required.")
				.EmailAddress()
				.WithMessage("A valid email is required.")
				.MaximumLength(100)
				.WithMessage("Email must not exceed 100 characters.");
			RuleFor(v => v.SkillSets)
				.NotEmpty()
				.WithMessage("SkillSets is required.")
				.MaximumLength(255)
				.WithMessage("SkillSets must not exceed 255 characters.");
			RuleFor(v => v.Hobby)
				.NotEmpty()
				.WithMessage("Hobby is required.")
				.MaximumLength(255)
				.WithMessage("Hobby must not exceed 255 characters.");
		}
	}

	public class AddEditContactCommandHandler(
		IMapper mapper, 
		IUnitOfWork<int> unitOfWork,
		ILogger<AddEditContactCommandHandler> logger,
		IAppCache appCache
		) : IRequestHandler<AddEditContactCommand, IResult<int>>
	{
		public async Task<IResult<int>> Handle(AddEditContactCommand command, CancellationToken cancellationToken)
		{
			if (command.Id == 0)
			{
				// Only add if max count is not more than 100.
				var count = await unitOfWork.Repository<Contact>().CountAsync();
                if (count > 100)
                {
					return await Result<int>.FailAsync("Max item count reached. Please delete some first.");
                }

                var contact = mapper.Map<Contact>(command);
				await unitOfWork.Repository<Contact>().AddAsync(contact);
				await unitOfWork.Commit(cancellationToken);

				// Invalidate cache.
				await appCache.RemoveAsync("contact_all");

				logger.LogInformation("New contact added with ID: {Id}", contact.Id);

				return await Result<int>.SuccessAsync(contact.Id, "New contact saved.");
			}
			else
			{
				var contact = await unitOfWork.Repository<Contact>().GetByIdAsync(command.Id);
				if (contact != null)
				{
					mapper.Map(command, contact);

					await unitOfWork.Repository<Contact>().UpdateAsync(contact);
					await unitOfWork.Commit(cancellationToken);

					// Invalidate cache.
					await appCache.RemoveAsync($"contact_{command.Id}");
					await appCache.RemoveAsync("contact_all");

					logger.LogInformation("Contact updated with ID: {Id}", contact.Id);

					return await Result<int>.SuccessAsync(contact.Id, "Contact updated.");
				}
				else
				{
					logger.LogWarning("Contact not found with ID: {Id}", command.Id);

					return await Result<int>.FailAsync("Contact not found!");
				}
			}
		}
	}
}
