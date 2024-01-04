using Sufi.Demo.PeopleDirectory.Domain.Contracts;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Models.Audit
{
	public class Audit : IEntity<int>
	{
		public int Id { get; set; }
		public string? UserId { get; set; }
		public string Type { get; set; } = null!;
		public string TableName { get; set; } = null!;
		public DateTime DateTime { get; set; }
		public string? OldValues { get; set; }
		public string? NewValues { get; set; }
		public string? AffectedColumns { get; set; }
		public string PrimaryKey { get; set; } = null!;
	}
}
