using Microsoft.AspNetCore.Identity;
using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity
{
	public class AppUser : IdentityUser<string>, IAuditableEntity<string>
	{
		public bool IsActive { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedOn { get; set; }
		public string? CreatedBy { get; set; }
		public DateTime CreatedOn { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModifiedOn { get; set; }
	}
}
