using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Services;
using Sufi.Demo.PeopleDirectory.Domain.Contracts;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;
using Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Contexts
{
	public class ApplicationDbContext : AuditableContext
	{
		private readonly ICurrentUserService _currentUserService;

		public virtual DbSet<Contact> Contacts { get; set; } = null!;
		public virtual DbSet<ServerInfo> ServerInfos { get; set; } = null!;

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : base(options)
		{
			_currentUserService = currentUserService;
		}

		public override Task<int> SaveChangesAsync(string? userId = null, CancellationToken cancellationToken = default)
		{
			PopulateAuditRecords();

			return base.SaveChangesAsync(userId, cancellationToken);
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			PopulateAuditRecords();

			if (_currentUserService.UserId == null)
			{
				return await base.SaveChangesAsync(cancellationToken);
			}

			return await base.SaveChangesAsync(_currentUserService.UserId, cancellationToken);
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
				property.SetColumnType("nvarchar(128)");
			}

			base.OnModelCreating(builder);

			builder.Entity<AppUser>(entity =>
			{
				entity.ToTable("Users");
				entity.Property(e => e.Id).ValueGeneratedOnAdd();
			});

			builder.Entity<AppRole>(entity =>
			{
				entity.ToTable("Roles");
			});

			builder.Entity<IdentityUserRole<string>>(entity =>
			{
				entity.ToTable("UserRoles");
			});

			builder.Entity<IdentityUserClaim<string>>(entity =>
			{
				entity.ToTable("UserClaims");
			});

			builder.Entity<IdentityUserLogin<string>>(entity =>
			{
				entity.ToTable("UserLogins");
			});

			builder.Entity<AppRoleClaim>(entity =>
			{
				entity.ToTable("RoleClaims");
			});

			builder.Entity<IdentityUserToken<string>>(entity =>
			{
				entity.ToTable("UserTokens");
			});
		}

		private void PopulateAuditRecords()
		{
			foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
			{
				switch (entry.State)
				{
					case EntityState.Added:
						entry.Entity.CreatedOn = DateTime.UtcNow;
						entry.Entity.CreatedBy = _currentUserService.UserId;
						break;

					case EntityState.Modified:
						entry.Entity.LastModifiedOn = DateTime.UtcNow;
						entry.Entity.LastModifiedBy = _currentUserService.UserId;
						break;
				}
			}
		}
	}
}
