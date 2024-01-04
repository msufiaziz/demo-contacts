using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.Domain.Entities.Misc
{
	public class Contact : AuditableEntity<int>
	{
		[Required]
		public string UserName { get; set; } = null!;
		[Required]
		[Phone]
		public string Phone { get; set; } = null!;
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		public string SkillSets { get; set; } = null!;
		[Required]
		public string Hobby { get; set; } = null!;
	}
}
