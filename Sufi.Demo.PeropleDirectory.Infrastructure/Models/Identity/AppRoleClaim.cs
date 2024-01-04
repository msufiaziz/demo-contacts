using Microsoft.AspNetCore.Identity;
using Sufi.Demo.PeopleDirectory.Domain.Contracts;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity
{
	public class AppRoleClaim : IdentityRoleClaim<string>, IAuditableEntity<int>
	{
		public string? Description { get; set; }
		public string? Group { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModifiedOn { get; set; }

		public AppRoleClaim() : base() { }

		public AppRoleClaim(string? description = null, string? group = null) : base()
		{
			Description = description;
			Group = group;
		}
	}
}
