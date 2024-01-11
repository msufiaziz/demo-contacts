using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v2
{
	[ApiVersion(2.0)]
	[Route("api/v{version:apiVersion}/infra")]
	public class Infra2Controller : BaseApiController<Infra2Controller>
	{
		[Route("ping")]
		[HttpGet]
		public IActionResult Ping() => Ok("Server is OK");
	}
}
