using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.Libs.DataContext.Entities
{
	public class ServerInfo
	{
		[Key]
		public string Key { get; set; } = null!;
		[Required]
		public string Value { get; set; } = null!;
	}
}
