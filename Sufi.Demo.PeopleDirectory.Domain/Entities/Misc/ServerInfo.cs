using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sufi.Demo.PeopleDirectory.Domain.Entities.Misc
{
	public class ServerInfo : AuditableEntity<string>
	{
		[Required]
		[Column(TypeName = "character varying(255)")]
		public string Value { get; set; } = null!;
	}
}
