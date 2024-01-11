using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v1
{
	[ApiVersion(1.0)]
	[Route("api/v{version:apiVersion}/[controller]")]
	public class InfraController : BaseApiController<InfraController>
	{
		[Route("ping")]
        [HttpGet]
        public IActionResult Ping() => Ok();
    }
}
