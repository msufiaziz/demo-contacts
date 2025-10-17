using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v2
{
	/// <summary>
	/// Provides API endpoints for infrastructure-related operations.
	/// </summary>
	/// <remarks>This controller is part of the infrastructure layer and includes endpoints for monitoring and diagnostics.</remarks>
	[ApiVersion(2.0)]
	public class InfraController : BaseApiController<InfraController>
	{
		/// <summary>
		/// Simulates a ping to check if the server is responsive.
		/// </summary>
		/// <returns></returns>
		[Route("ping")]
		[HttpGet]
		public IActionResult Ping() => Ok("Server is OK");
	}
}
