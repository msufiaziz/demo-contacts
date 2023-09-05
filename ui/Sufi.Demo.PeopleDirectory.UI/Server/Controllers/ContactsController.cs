using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Sufi.Demo.PeopleDirectory.Libs.DataContext;
using Sufi.Demo.PeopleDirectory.Libs.DataContext.Entities;
using Sufi.Demo.PeopleDirectory.UI.Shared;
using System.ComponentModel.DataAnnotations;

namespace Sufi.Demo.PeopleDirectory.UI.Server.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ContactsController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;

		public ContactsController(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			var result = new List<ContactModel>();

			try
			{
				_context.Contacts.ToList().ForEach(c =>
				{
					result.Add(_mapper.Map<ContactModel>(c));
				});

				return Ok(result);
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}

		[HttpGet]
		public ContactModel Get(int id)
		{
			var contact = _context.Contacts.Find(id);
			return _mapper.Map<ContactModel>(contact);
		}

		[HttpPost]
		public IActionResult Create(CreateContactRequest request)
		{
			var newContact = _mapper.Map<Contact>(request);

			_context.Contacts.Add(newContact);
			_context.SaveChanges();

			return Ok();
		}

		[HttpPost]
		public IActionResult Update(UpdateContactRequest request)
		{
			var contact = _context.Contacts.Find(request.Id);
			if (contact != null)
			{
				contact.Email = request.Email;
				contact.Hobby = request.Hobby;
				contact.Phone = request.Phone;
				contact.SkillSets = request.SkillSets;
				contact.UserName = request.UserName;
				_context.SaveChanges();

				return Ok();
			}

			return NotFound();
		}

		[HttpPost]
		public IActionResult Delete([Required] int id)
		{
			var contact = _context.Contacts.Find(id);
			if (contact != null)
			{
				_context.Contacts.Remove(contact);
				_context.SaveChanges();
			}

			return Ok();
		}
	}
}
