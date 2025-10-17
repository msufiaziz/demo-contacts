using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v1
{
	/// <summary>
	/// Provides API endpoints for infrastructure-related operations.
	/// </summary>
	/// <remarks>This controller is part of the infrastructure layer and includes endpoints for monitoring and diagnostics.</remarks>
	[ApiVersion(1.0)]
	public class InfraController(
		ILogger<InfraController> logger
		) : BaseApiController<InfraController>
	{
		/// <summary>
		/// Simulates a ping to check if the server is responsive.
		/// </summary>
		/// <returns></returns>
		[Route("ping")]
        [HttpGet]
        public IActionResult Ping()
		{
			logger.LogInformation("InfraController.Ping method called.");

			return Ok();
		}
    }
}
