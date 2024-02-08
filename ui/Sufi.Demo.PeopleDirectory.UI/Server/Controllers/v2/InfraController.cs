using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v2
{
	[ApiVersion(2.0)]
	public class InfraController : BaseApiController<InfraController>
	{
		[Route("ping")]
		[HttpGet]
		public IActionResult Ping() => Ok("Server is OK");
	}
}
