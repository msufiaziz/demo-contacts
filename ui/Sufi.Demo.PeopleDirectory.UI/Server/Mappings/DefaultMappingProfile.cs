using AutoMapper;
using Sufi.Demo.PeopleDirectory.Application.Responses;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.UI.Shared;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Mappings
{
	public class DefaultMappingProfile : Profile
	{
		public DefaultMappingProfile() 
		{
			CreateMap<Contact, ContactModel>();
			CreateMap<Contact, ContactResponse>();
		}
	}
}
