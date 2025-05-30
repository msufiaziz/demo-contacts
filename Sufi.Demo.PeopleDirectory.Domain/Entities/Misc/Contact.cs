using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sufi.Demo.PeopleDirectory.Domain.Entities.Misc
{
	public class Contact : AuditableEntity<int>
	{
		[Required]
		[Column(TypeName = "character varying(50)")]
		public string UserName { get; set; } = null!;
		[Required]
		[Phone]
		[Column(TypeName = "character varying(20)")]
		public string Phone { get; set; } = null!;
		[Required]
		[EmailAddress]
		[Column(TypeName = "character varying(100)")]
		public string Email { get; set; } = null!;
		[Required]
		[Column(TypeName = "character varying(255)")]
		public string SkillSets { get; set; } = null!;
		[Required]
		[Column(TypeName = "character varying(255)")]
		public string Hobby { get; set; } = null!;
	}
}
