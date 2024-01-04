using AutoMapper;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetAll;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;

namespace Sufi.Demo.PeopleDirectory.Application.Mappings
{
	public class ContactProfile : Profile
	{
		public ContactProfile() 
		{ 
			CreateMap<AddEditContactCommand, Contact>().ReverseMap();
			CreateMap<GetAllContactsResponse, Contact>().ReverseMap();
		}
	}
}
