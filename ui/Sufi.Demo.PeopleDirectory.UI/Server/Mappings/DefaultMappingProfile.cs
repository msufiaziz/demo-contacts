using AutoMapper;
using Sufi.Demo.PeopleDirectory.Libs.DataContext.Entities;
using Sufi.Demo.PeopleDirectory.UI.Shared;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Mappings
{
	public class DefaultMappingProfile : Profile
	{
		public DefaultMappingProfile() 
		{
			CreateMap<Contact, ContactModel>();
			CreateMap<CreateContactRequest, Contact>();
		}
	}
}
