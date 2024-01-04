using Sufi.Demo.PeopleDirectory.Application.Interfaces.Services;
using System.Security.Claims;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Services
{
	public class CurrentUserService : ICurrentUserService
	{
		public string? UserId { get; }
		public List<KeyValuePair<string, string>>? Claims { get; set; }

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
		}
	}
}
