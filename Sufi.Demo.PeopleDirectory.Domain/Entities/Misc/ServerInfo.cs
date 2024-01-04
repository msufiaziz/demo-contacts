using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.Domain.Entities.Misc
{
	public class ServerInfo : AuditableEntity<string>
	{
		[Required]
		public string Value { get; set; } = null!;
	}
}
