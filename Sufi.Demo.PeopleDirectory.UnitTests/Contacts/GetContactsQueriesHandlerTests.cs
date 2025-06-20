using AutoMapper;
using Moq;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetAll;
using Sufi.Demo.PeopleDirectory.Application.Features.Contacts.Queries.GetById;
using Sufi.Demo.PeopleDirectory.Application.Interfaces.Repositories;
using Sufi.Demo.PeopleDirectory.Domain.Entities.Misc;

namespace Sufi.Demo.PeopleDirectory.UnitTests.Contacts
{
    public class GetContactsQueriesHandlerTests
    {
        private readonly Mock<IUnitOfWork<int>> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;

        public GetContactsQueriesHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork<int>>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetAllContactsQueryHandler_ReturnsMappedContacts()
        {
            // Arrange
            var contacts = new List<Contact>
            {
                new() { Id = 1, UserName = "User1", Phone = "123", Email = "user1@example.com", SkillSets = "C#", Hobby = "Reading" },
                new() { Id = 2, UserName = "User2", Phone = "456", Email = "user2@example.com", SkillSets = "Java", Hobby = "Swimming" }
            };
            var mappedContacts = new List<GetAllContactsResponse>
            {
                new() { Id = 1, UserName = "User1", Phone = "123", Email = "user1@example.com", SkillSets = "C#", Hobby = "Reading" },
                new() { Id = 2, UserName = "User2", Phone = "456", Email = "user2@example.com", SkillSets = "Java", Hobby = "Swimming" }
            };

            _unitOfWorkMock.Setup(u => u.Repository<Contact>().GetAllAsync()).ReturnsAsync(contacts);
            _mapperMock.Setup(m => m.Map<List<GetAllContactsResponse>>(contacts)).Returns(mappedContacts);

            var handler = new GetAllContactsQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(new GetAllContactsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Equal("User1", result.Data[0].UserName);
            Assert.Equal("User2", result.Data[1].UserName);
        }

        [Fact]
        public async Task GetContactByIdQueryHandler_ReturnsMappedContact_WhenContactExists()
        {
            // Arrange
            var contact = new Contact { Id = 1, UserName = "User1", Phone = "123", Email = "user1@example.com", SkillSets = "C#", Hobby = "Reading" };
            var mappedContact = new GetContactByIdResponse { Id = 1, UserName = "User1", Phone = "123", Email = "user1@example.com", SkillSets = "C#", Hobby = "Reading" };

            _unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(1)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<GetContactByIdResponse>(contact)).Returns(mappedContact);

            var handler = new GetContactByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(new GetContactByIdQuery { Id = 1 }, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal("User1", result.Data.UserName);
        }

        [Fact]
        public async Task GetContactByIdQueryHandler_ReturnsNullData_WhenContactDoesNotExist()
        {
            // Arrange
            Contact? contact = null;
            GetContactByIdResponse? mappedContact = null;

            _unitOfWorkMock.Setup(u => u.Repository<Contact>().GetByIdAsync(99)).ReturnsAsync(contact);
            _mapperMock.Setup(m => m.Map<GetContactByIdResponse>(contact)).Returns(mappedContact);

            var handler = new GetContactByIdQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);

            // Act
            var result = await handler.Handle(new GetContactByIdQuery { Id = 99 }, CancellationToken.None);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Data);
        }
    }
}
