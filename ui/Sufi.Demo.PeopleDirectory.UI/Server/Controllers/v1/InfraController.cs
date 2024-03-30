using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v1
{
	[ApiVersion(1.0)]
	public class InfraController : BaseApiController<InfraController>
	{
		private readonly ILogger<InfraController> _logger;

		public InfraController(ILogger<InfraController> logger)
		{
			_logger = logger;
		}

		[Route("ping")]
        [HttpGet]
        public IActionResult Ping()
		{
			_logger.LogInformation("InfraController.Ping method called.");

			return Ok();
		}
    }
}
