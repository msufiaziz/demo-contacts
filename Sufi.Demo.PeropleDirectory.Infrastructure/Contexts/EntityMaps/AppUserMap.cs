using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sufi.Demo.PeropleDirectory.Infrastructure.Models.Identity;

namespace Sufi.Demo.PeropleDirectory.Infrastructure.Contexts.EntityMaps
{
	public class AppUserMap : IEntityTypeConfiguration<AppUser>
	{
		public void Configure(EntityTypeBuilder<AppUser> builder)
		{
			builder.ToTable("Users", "Identity");

			builder.Property(e => e.Id).ValueGeneratedOnAdd();
		}
	}
}
