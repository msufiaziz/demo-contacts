using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.Domain.Common
{
	public interface IEntity
	{
	}

	public interface IEntity<TId> : IEntity 
	{
		[Key]
		TId Id { get; set; }
	}
}
