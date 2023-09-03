using Microsoft.EntityFrameworkCore;
using Sufi.Demo.PeopleDirectory.Libs.DataContext.Entities;

namespace Sufi.Demo.PeopleDirectory.Libs.DataContext
{
	public class AppDbContext : DbContext
	{
		public virtual DbSet<Contact> Contacts { get; set; } = null!;

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
	}
}
