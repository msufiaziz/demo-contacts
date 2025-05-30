using Microsoft.AspNetCore.Identity;
using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity
{
	public class AppRole : IdentityRole, IAuditableEntity<string>
	{
		[Column(TypeName = "character varying(100)")]
		public string? Description { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModifiedOn { get; set; }
		public virtual ICollection<AppRoleClaim> RoleClaims { get; set; }

		public AppRole() : base()
		{
			RoleClaims = new HashSet<AppRoleClaim>();
		}

		public AppRole(string roleName, string? description = null) : base(roleName)
		{
			RoleClaims = new HashSet<AppRoleClaim>();
			Description = description;
		}
	}
}
