using Microsoft.AspNetCore.Http;
using Sufi.Demo.PeopleDirectory.Application.Contracts.Services;
using System.Security.Claims;

namespace Sufi.Demo.PeopleDirectory.Infrastructure.Identity
{
	public class CurrentUserService : ICurrentUserService
	{
		public string? UserId { get; }
		public List<KeyValuePair<string, string>>? Claims { get; set; }

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			var userClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
			if (userClaim != null)
			{
				UserId = userClaim.Value;
			}

			Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
		}
	}
}
