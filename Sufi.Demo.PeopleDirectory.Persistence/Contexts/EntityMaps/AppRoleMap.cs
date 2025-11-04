using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sufi.Demo.PeopleDirectory.Persistence.Models.Identity;

namespace Sufi.Demo.PeopleDirectory.Persistence.Contexts.EntityMaps
{
	public class AppRoleMap : IEntityTypeConfiguration<AppRole>
	{
		public void Configure(EntityTypeBuilder<AppRole> builder)
		{
			builder.ToTable("Roles", "Identity");
		}
	}
}
