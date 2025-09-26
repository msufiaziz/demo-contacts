using Microsoft.Extensions.Logging;
using Moq;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Commands;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;

namespace Sufi.Demo.PeopleDirectory.UnitTests.Contacts
{
    public class DeleteContactCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork<int>> _unitOfWorkMock;
		private readonly Mock<ILogger<DeleteContactCommandHandler>> _loggerMock = new();
		private readonly DeleteContactCommandHandler _handler;

        public DeleteContactCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<int>>();
            _handler = new DeleteContactCommandHandler(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldDeleteContact_WhenContactExists()
        {
            // Arrange
            var command = new DeleteContactCommand { Id = 1 };
            var existingContact = new Contact { Id = 1 };
            _unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(1)).ReturnsAsync(existingContact);
            _unitOfWorkMock.Setup(u => u.Repository<Contact>().DeleteAsync(existingContact)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.Commit(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Empty(result.Messages); // Success messages are empty in this implementation
        }

        [Fact]
        public async Task Handle_ShouldFailToDeleteContact_WhenContactDoesNotExist()
        {
            // Arrange
            var command = new DeleteContactCommand { Id = 1 };
			Contact? existingContact = null;
			_unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(1)).ReturnsAsync(existingContact);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("No data to delete.", result.Messages[0]);
        }
    }
}
