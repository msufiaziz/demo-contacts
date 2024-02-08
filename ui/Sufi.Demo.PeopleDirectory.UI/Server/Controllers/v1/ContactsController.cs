using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetAll;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetById;
using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v1
{
    /// <summary>
    /// A controller for manipulating contact data.
    /// </summary>
	[ApiVersion(1.0)]
	public class ContactsController : BaseApiController<ContactsController>
    {

		/// <summary>
		/// Initialize an instance of <see cref="ContactsController"/> class.
		/// </summary>
		public ContactsController()
        {
        }

        /// <summary>
        /// Get all contacts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllContactsQuery()));
        }

        /// <summary>
        /// Get a contact by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await  Mediator.Send(new GetContactByIdQuery { Id = id }));

        /// <summary>
        /// Create/Update a contact.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(AddEditContactCommand request) => Ok(await Mediator.Send(request));

        /// <summary>
        /// Delete a contact.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([Required] int id)
        {
            var command = new DeleteContactCommand { Id = id };
            return Ok(await Mediator.Send(command));
		}
    }
}
