using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sufi.Demo.PeopleDirectory.Persistence.Models.Identity;

namespace Sufi.Demo.PeopleDirectory.Persistence.Contexts.EntityMaps
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
