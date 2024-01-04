namespace Sufi.Demo.PeopleDirectory.Application.Responses
{
	public class ContactResponse
	{
		public int Id { get; set; }
		public string UserName { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string SkillSets { get; set; } = null!;
		public string Hobby { get; set; } = null!;
	}
}
