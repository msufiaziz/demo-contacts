using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using Sufi.Demo.PeopleDirectory.Domain.Common;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeopleDirectory.Persistence.Models.Identity;

namespace Sufi.Demo.PeopleDirectory.Persistence.Contexts
{
	public class ApplicationDbContext(
		DbContextOptions<ApplicationDbContext> options, 
		ICurrentUserService currentUserService
		) : AuditableContext(options)
	{
		public virtual DbSet<Contact> Contacts { get; set; } = null!;
		public virtual DbSet<ServerInfo> ServerInfos { get; set; } = null!;

		public override Task<int> SaveChangesAsync(string? userId = null, CancellationToken cancellationToken = default)
		{
			PopulateAuditRecords();

			return base.SaveChangesAsync(userId, cancellationToken);
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			PopulateAuditRecords();

			if (currentUserService.UserId == null)
			{
				return await base.SaveChangesAsync(cancellationToken);
			}

			return await base.SaveChangesAsync(currentUserService.UserId, cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			foreach (var property in builder.Model.GetEntityTypes()
				.SelectMany(t => t.GetProperties())
				.Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
			{
				property.SetColumnType("decimal(18,2)");
			}

			foreach (var property in builder.Model.GetEntityTypes()
				.SelectMany(t => t.GetProperties())
				.Where(p => p.Name is "LastModifiedBy" or "CreatedBy"))
			{
				property.SetColumnType("character varying(100)");
			}

			base.OnModelCreating(builder);

			var assembly = typeof(ApplicationDbContext).Assembly;
			builder.ApplyConfigurationsFromAssembly(assembly);

			builder.Entity<AppRole>(entity => entity.ToTable("Roles", "Identity"));

			builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("UserRoles", "Identity"));

			builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("UserClaims", "Identity"));

			builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("UserLogins", "Identity"));

			builder.Entity<AppRoleClaim>(entity => entity.ToTable("RoleClaims", "Identity"));

			builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("UserTokens", "Identity"));
		}

		private void PopulateAuditRecords()
		{
			foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.Entity.CreatedOn = DateTime.UtcNow;
						entry.Entity.CreatedBy = currentUserService.UserId;
						break;

					case EntityState.Modified:
						entry.Entity.LastModifiedOn = DateTime.UtcNow;
						entry.Entity.LastModifiedBy = currentUserService.UserId;
						break;
				}
			}
		}
	}
}
