using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Models.Audit
{
	public class Audit : IEntity<int>
	{
		public int Id { get; set; }
		[Column(TypeName = "character varying(100)")]
		public string? UserId { get; set; }
		[Column(TypeName = "character varying(20)")]
		public string Type { get; set; } = null!;
		[Column(TypeName = "character varying(50)")]
		public string TableName { get; set; } = null!;
		public DateTime DateTime { get; set; }
		[Column(TypeName = "character varying(255)")]
		public string? OldValues { get; set; }
		[Column(TypeName = "character varying(255)")]
		public string? NewValues { get; set; }
		[Column(TypeName = "character varying(100)")]
		public string? AffectedColumns { get; set; }
		[Column(TypeName = "character varying(100)")]
		public string PrimaryKey { get; set; } = null!;
	}
}
