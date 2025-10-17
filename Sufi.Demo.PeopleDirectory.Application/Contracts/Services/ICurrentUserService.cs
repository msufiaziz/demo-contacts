using Sufi.Demo.PeopleDirectory.Application.Contracts.Common;

namespace Sufi.Demo.PeopleDirectory.Application.Contracts.Services
{
	public interface ICurrentUserService : IService
	{
		string? UserId { get; }
	}
}
