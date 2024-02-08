namespace Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetById
{
	public class GetContactByIdResponse
	{
		public int Id { get; set; }
		public string UserName { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string SkillSets { get; set; } = null!;
		public string Hobby { get; set; } = null!;
	}
}
