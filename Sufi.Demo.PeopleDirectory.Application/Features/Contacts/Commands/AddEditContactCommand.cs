using AutoMapper;
using MediatR;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Shared.Wrapper;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands
{
	public class AddEditContactCommand : IRequest<IResult<int>>
	{
		public int Id { get; set; }
		[Required]
		public string UserName { get; set; } = "username";
		[Required]
		[Phone]
		public string Phone { get; set; } = "01234567890";
		[Required]
		[EmailAddress]
		public string Email { get; set; } = "user@example.com";
		[Required]
		public string SkillSets { get; set; } = "skill1, skill2, skill3";
		[Required]
		public string Hobby { get; set; } = "Hobby";
	}

	internal class AddEditContactCommandHandler : IRequestHandler<AddEditContactCommand, IResult<int>>
	{
		private readonly IMapper _mapper;
		private readonly IUnitOfWork<int> _unitOfWork;

		public AddEditContactCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public async Task<IResult<int>> Handle(AddEditContactCommand command, CancellationToken cancellationToken)
		{
			if (command.Id == 0)
			{
				// Only add if max count is not more than 100.
				var count = await _unitOfWork.Repository<Contact>().CountAsync();
                if (count > 100)
                {
					return await Result<int>.FailAsync("Max item count reached. Please delete some first.");
                }

                var contact = _mapper.Map<Contact>(command);
				await _unitOfWork.Repository<Contact>().AddAsync(contact);
				await _unitOfWork.Commit(cancellationToken);
				return await Result<int>.SuccessAsync(contact.Id, "New contact saved.");
			}
			else
			{
				var contact = await _unitOfWork.Repository<Contact>().GetByIdAsync(command.Id);
				if (contact != null)
				{
					contact.Email = command.Email;
					contact.Hobby = command.Hobby;
					contact.Phone = command.Phone;
					contact.UserName = command.UserName;
					contact.SkillSets = command.SkillSets;
					await _unitOfWork.Repository<Contact>().UpdateAsync(contact);
					await _unitOfWork.Commit(cancellationToken);
					return await Result<int>.SuccessAsync(contact.Id, "Contact updated.");
				}
				else
				{
					return await Result<int>.FailAsync("Contact not found!");
				}
			}
		}
	}
}
