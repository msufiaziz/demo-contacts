using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sufi.Demo.PeopleDirectory.Application.Enums;
using Sufi.Demo.PeropleDirectory.Infrastructure.Models.Audit;
using Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Contexts
{
	public abstract class AuditableContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, AppRoleClaim, IdentityUserToken<string>>
	{
		protected AuditableContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Audit> AuditTrails { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Audit>().ToTable("AuditTrails", "Audit");
		}

		public virtual async Task<int> SaveChangesAsync(string? userId = null, CancellationToken cancellationToken = new())
		{
			var auditEntries = OnBeforeSaveChanges(userId);
			var result = await base.SaveChangesAsync(cancellationToken);
			await OnAfterSaveChanges(auditEntries, cancellationToken);
			return result;
		}

		private List<AuditEntry> OnBeforeSaveChanges(string? userId)
		{
			ChangeTracker.DetectChanges();
			var auditEntries = new List<AuditEntry>();
			foreach (var entry in ChangeTracker.Entries())
			{
				if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
					continue;

				var auditEntry = new AuditEntry(entry)
				{
					TableName = entry.Entity.GetType().Name,
					UserId = userId
				};
				auditEntries.Add(auditEntry);
				foreach (var property in entry.Properties)
				{
					if (property.IsTemporary)
					{
						auditEntry.TemporaryProperties.Add(property);
						continue;
					}

					string propertyName = property.Metadata.Name;
					if (property.Metadata.IsPrimaryKey())
					{
						auditEntry.KeyValues[propertyName] = property.CurrentValue;
						continue;
					}

					switch (entry.State)
					{
						case EntityState.Added:
							auditEntry.AuditType = AuditType.Create;
							auditEntry.NewValues[propertyName] = property.CurrentValue;
							break;

						case EntityState.Deleted:
							auditEntry.AuditType = AuditType.Delete;
							auditEntry.OldValues[propertyName] = property.OriginalValue;
							break;

						case EntityState.Modified:
							if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
							{
								auditEntry.ChangedColumns.Add(propertyName);
								auditEntry.AuditType = AuditType.Update;
								auditEntry.OldValues[propertyName] = property.OriginalValue;
								auditEntry.NewValues[propertyName] = property.CurrentValue;
							}
							break;
					}
				}
			}
			foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
			{
				AuditTrails.Add(auditEntry.ToAudit());
			}
			return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
		}

		private Task OnAfterSaveChanges(List<AuditEntry> auditEntries, CancellationToken cancellationToken = new())
		{
			if (auditEntries == null || auditEntries.Count == 0)
				return Task.CompletedTask;

			foreach (var auditEntry in auditEntries)
			{
				foreach (var prop in auditEntry.TemporaryProperties)
				{
					if (prop.Metadata.IsPrimaryKey())
					{
						auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
					}
					else
					{
						auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
					}
				}
				AuditTrails.Add(auditEntry.ToAudit());
			}
			return SaveChangesAsync(cancellationToken);
		}
	}
}
