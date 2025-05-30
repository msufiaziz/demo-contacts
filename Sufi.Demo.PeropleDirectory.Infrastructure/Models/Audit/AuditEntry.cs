﻿using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sufi.Demo.PeopleDirectory.Application.Enums;
using System.Text.Json;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Models.Audit
{
	public class AuditEntry(EntityEntry entry)
	{
		public EntityEntry Entry { get; } = entry;
		public string? UserId { get; set; }
		public string TableName { get; set; } = null!;
		public Dictionary<string, object?> KeyValues { get; } = new();
		public Dictionary<string, object?> OldValues { get; } = new();
		public Dictionary<string, object?> NewValues { get; } = new();
		public List<PropertyEntry> TemporaryProperties { get; } = new();
		public AuditType AuditType { get; set; }
		public List<string> ChangedColumns { get; } = new();
		public bool HasTemporaryProperties => TemporaryProperties.Any();

		public Audit ToAudit()
		{
			var audit = new Audit
			{
				UserId = UserId,
				Type = AuditType.ToString(),
				TableName = TableName,
				DateTime = DateTime.UtcNow,
				PrimaryKey = JsonSerializer.Serialize(KeyValues),
				OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
				NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
				AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns)
			};
			return audit;
		}
	}
}
