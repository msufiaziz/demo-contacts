using Microsoft.AspNetCore.Identity;
using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity
{
	public class AppRoleClaim : IdentityRoleClaim<string>, IAuditableEntity<int>
	{
		[Column(TypeName = "character varying(100)")]
		public string? Description { get; set; }
		[Column(TypeName = "character varying(100)")]
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
