using AutoMapper;
using Moq;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;

namespace Sufi.Demo.PeopleDirectory.UnitTests.Contacts
{
	public class AddEditContactCommandHandlerTests
	{
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IUnitOfWork<int>> _unitOfWorkMock;
		private readonly AddEditContactCommandHandler _handler;

		public AddEditContactCommandHandlerTests()
		{
			_mapperMock = new Mock<IMapper>();
			_unitOfWorkMock = new Mock<IUnitOfWork<int>>();
			_handler = new AddEditContactCommandHandler(_mapperMock.Object, _unitOfWorkMock.Object);
		}

		[Fact]
		public async Task Handle_ShouldAddNewContact_WhenMaxCountNotExceeded()
		{
			// Arrange
			var command = new AddEditContactCommand
			{
				Id = 0,
				UserName = "JohnDoe",
				Email = "john@example.com",
				Phone = "1234567890",
				SkillSets = "C#, SQL",
				Hobby = "Reading"
			};
			_unitOfWorkMock.Setup(u => u.Repository<Contact>().CountAsync()).ReturnsAsync(50);
			_mapperMock.Setup(m => m.Map<Contact>(command)).Returns(new Contact());
			_unitOfWorkMock.Setup(u => u.Repository<Contact>().AddAsync(It.IsAny<Contact>())).Returns(Task.FromResult(new Contact()));
			_unitOfWorkMock.Setup(u => u.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.Succeeded);
			Assert.Equal("New contact saved.", result.Messages[0]);
		}

		[Fact]
		public async Task Handle_ShouldFailToAddNewContact_WhenMaxCountExceeded()
		{
			// Arrange
			var command = new AddEditContactCommand { Id = 0 };
			_unitOfWorkMock.Setup(u => u.Repository<Contact>().CountAsync()).ReturnsAsync(101);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.Succeeded);
			Assert.Equal("Max item count reached. Please delete some first.", result.Messages[0]);
		}

		[Fact]
		public async Task Handle_ShouldUpdateContact_WhenContactExists()
		{
			// Arrange
			var command = new AddEditContactCommand
			{
				Id = 1,
				UserName = "JaneDoe",
				Email = "jane@example.com",
				Phone = "0987654321",
				SkillSets = "Java, Python",
				Hobby = "Traveling"
			};
			var existingContact = new Contact { Id = 1 };
			_unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(1)).ReturnsAsync(existingContact);
			_unitOfWorkMock.Setup(u => u.Repository<Contact>().UpdateAsync(existingContact)).Returns(Task.CompletedTask);
			_unitOfWorkMock.Setup(u => u.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.Succeeded);
			Assert.Equal("Contact updated.", result.Messages[0]);
		}

		[Fact]
		public async Task Handle_ShouldFailToUpdateContact_WhenContactDoesNotExist()
		{
			// Arrange
			var command = new AddEditContactCommand { Id = 1 };
			Contact? existingContact = null;
			_unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(1)).ReturnsAsync(existingContact);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.False(result.Succeeded);
			Assert.Equal("Contact not found!", result.Messages[0]);
		}
	}
}