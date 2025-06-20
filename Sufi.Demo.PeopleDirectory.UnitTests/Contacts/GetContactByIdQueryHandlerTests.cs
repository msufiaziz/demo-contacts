using AutoMapper;
using Moq;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetById;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;

namespace Sufi.Demo.PeopleDirectory.UnitTests.Contacts
{
    public class GetContactByIdQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork<int>> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetContactByIdQueryHandler _handler;

        public GetContactByIdQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<int>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetContactByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsMappedContact_WhenContactExists()
        {
            // Arrange
            var contact = new Contact
            {
                Id = 1,
                UserName = "User1",
                Phone = "123",
                Email = "user1@example.com",
                SkillSets = "C#",
                Hobby = "Reading"
            };
            var mappedContact = new GetContactByIdResponse
            {
                Id = 1,
                UserName = "User1",
                Phone = "123",
                Email = "user1@example.com",
                SkillSets = "C#",
                Hobby = "Reading"
            };

            _unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(1)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<GetContactByIdResponse>(contact)).Returns(mappedContact);

            var query = new GetContactByIdQuery { Id = 1 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("User1", result.Data.UserName);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenContactDoesNotExist()
        {
            // Arrange
            Contact? contact = null;
            GetContactByIdResponse? mappedContact = null;

            _unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(99)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<GetContactByIdResponse>(contact)).Returns(mappedContact);

            var query = new GetContactByIdQuery { Id = 99 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Data);
        }
    }
}