using Sufi.Demo.PeopleDirectory.Application.Interfaces.Common;

namespace Sufi.Demo.PeopleDirectory.Application.Interfaces.Services
{
	public interface ICurrentUserService : IService
	{
		string? UserId { get; }
	}
}
