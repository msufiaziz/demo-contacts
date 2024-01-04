using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetAll;
using Sufi.Demo.PeopleDirectory.UI.Shared;
using Sufi.Demo.PeropleDirectory.Infrastructure.Contexts;
using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers.v1
{
    [ApiVersion(2.0)]
	[Route("api/v{version:apiVersion}/contacts")]
	public class Contacts2Controller : BaseApiController<Contacts2Controller>
    {
        [HttpGet]
        public IActionResult Get() => Ok();
    }

    /// <summary>
    /// 
    /// </summary>
	[ApiVersion(1.0)]
	public class ContactsController : BaseApiController<ContactsController>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

		/// <summary>
		/// Initialize an instance of <see cref="ContactsController"/> class.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="mapper"></param>
		public ContactsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all contacts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contacts = await Mediator.Send(new GetAllContactsQuery());
            return Ok(contacts);
        }

        /// <summary>
        /// Get a contact by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ContactModel Get(int id)
        {
            var contact = _context.Contacts.Find(id);
            return _mapper.Map<ContactModel>(contact);
        }

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
