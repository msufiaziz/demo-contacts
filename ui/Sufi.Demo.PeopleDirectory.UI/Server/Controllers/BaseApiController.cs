using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers
{
	/// <summary>
	/// Represent the base controller class.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[ApiController]
	[Route("api/v{version:apiVersion}/[controller]")]
	public abstract class BaseApiController<T> : ControllerBase
	{
		private IMediator? _mediatorInstance;
		private ILogger<T>? _loggerInstance;

		/// <summary>
		/// Gets the mediator for requests/responses.
		/// </summary>
		protected IMediator Mediator => _mediatorInstance ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
		protected ILogger<T> Logger => _loggerInstance ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
	}
}
