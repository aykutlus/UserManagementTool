using ManagementTool.Functions.Domain;
using ManagementTool.Functions.Infrastructure;
using ManagementTool.Functions.Presentation.Models;
using Moq;

namespace ManagementTool.Functions.Tests.Application
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GivenValidRequest_WHenGetUsersAsync_ThenReturnsAllUsers()
        {
            //arrange
            var users = new List<DomainUser>
            {
                DomainUser.CreateWithIdAndCredits("1", "User1", "user1@example.com", 100),
                DomainUser.CreateWithIdAndCredits("2", "User2", "user2@example.com", 200)
            };
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            //act
            var result = await _userService.GetUsersAsync(includeDeleted: true);

            //assert
            Assert.Equal(users, result);
            _userRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GivenIncludeDeletedIsFalse_WhenGetUsersAsync_ThenExcludesDeletedUsers()
        {
            //arrange
            var user1 = DomainUser.CreateWithIdAndCredits("1", "User1", "user1@example.com", 100);
            var user2 = DomainUser.CreateWithIdAndCredits("2", "User2", "user2@example.com", 200);
            user2.AddHistoryEntry(ChangeType.Deleted, null, null);

            var users = new List<DomainUser> { user1, user2 };
            _userRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

            //act
            var result = await _userService.GetUsersAsync(includeDeleted: false);

            // Assert
            Assert.DoesNotContain(user2, result);
        }

        [Fact]
        public async Task GivenValidRequest_WHenGetUserByIdAsync_ThenCallsGetByIdAsync()
        {
            //arrange
            var user = DomainUser.CreateWithIdAndCredits("1", "User1", "user1@example.com", 100);
            var id = "1";
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(user);

            //act
            var result = await _userService.GetUserByIdAsync(id);

            //assert
            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GivenValidRequest_WhenCreateUserAsync_ThenCreatesUser()
        {
            //arrange
            var name = "New User";
            var email = "newuser@example.com";

            //act
            var result = await _userService.CreateUserAsync(name, email);

            //assert
            Assert.Equal(email, result.Email);
            Assert.Equal(name, result.Name);
            Assert.NotNull(result.Id);
        }

        [Fact]
        public async Task GivenUpdatingName_WhenUpdateUserDetailsAsync_ThenUpdatseUserDetails()
        {
            //arrange
            var user = DomainUser.CreateWithIdAndCredits("1", "Old Name", "old@example.com", 100);
            var id = "1";

            _userRepositoryMock.Setup(t => t.GetByIdAsync(id)).ReturnsAsync(user);

            var updateRequest = new UserUpdateRequest
            {
                Name = "New Name",
                Email = "new@example.com"
            };

            //act
            var result = await _userService.UpdateUserDetailsAsync(id, updateRequest);

            //assert
            Assert.Equal("New Name", result.Name);
            Assert.Equal("new@example.com", result.Email);
            Assert.Equal(result.History.Last().ChangeType,ChangeType.ModifiedUserDetails);

            _userRepositoryMock.Verify(repo => repo.GetByIdAsync(id), Times.Once);
            _userRepositoryMock.Verify(repo => repo.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task GivenValidRequest_WHenDeleteUserAsync_ThenCallsDeleteAsync()
        {
            //arrange
            var fakeUser = DomainUser.CreateWithIdAndCredits("1", "User1", "user1@example.com", 100);
            var fakeId = "1";
            _userRepositoryMock.Setup(repo => repo.DeleteAsync(fakeId)).ReturnsAsync(true);

            //act
            var result = await _userService.DeleteUserAsync(fakeId);

            //assert
            _userRepositoryMock.Verify(repo => repo.DeleteAsync(fakeId), Times.Once);
        }
    }
}
