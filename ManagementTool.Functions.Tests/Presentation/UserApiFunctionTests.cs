using ManagementTool.Functions.Domain;
using ManagementTool.Functions.Presentation;
using ManagementTool.Functions.Presentation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System.Text;

namespace ManagementTool.Functions.Tests.Presentation
{
    public class UserApiFunctionTests
    {
        private Mock<IUserService> _mockUserService;
        public UserApiFunctionTests()
        {
            _mockUserService = new();
        }

        [Fact]
        public async Task GivenNoUser_WhenListUsers_ThenReturnsEmptyList()
        {
            //arrange
            var emptyList = new List<DomainUser>();
            var fakeRequest = new Mock<HttpRequest>();
            fakeRequest.Setup(t => t.Query["deleted"]).Returns("false");

            _mockUserService.Setup(t => t.GetUsersAsync(false)).ReturnsAsync(new List<DomainUser>());

            //act
            var target = new UserApiFunctions(_mockUserService.Object);
            var result = await target.ListUsers(fakeRequest.Object);

            //asert
            Assert.Equal(emptyList, ((OkObjectResult)result).Value);
        }

        [Fact]
        public async Task GivenInvalidRequest_WhenCreateUser_ThenReturnsBadRequest()
        {
            //arrange
            UserCreationRequest fakeUser = new UserCreationRequest() { Name = "fake name" };
            var fakeRequest = new Mock<HttpRequest>();
            string fakeInput = JsonConvert.SerializeObject(fakeUser);
            Stream content = new MemoryStream(Encoding.UTF8.GetBytes(fakeInput));
            fakeRequest.Setup(t => t.Body).Returns(content);

            //act
            var target = new UserApiFunctions(_mockUserService.Object);
            var result = await target.CreateUser(fakeRequest.Object);

            //asert
            Assert.Equal(400, ((BadRequestObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task GivenValidRequest_WhenCreateUser_ThenReturnsOK()
        {
            //arrange
            UserCreationRequest fakeUser = new UserCreationRequest()
            {
                Email = "fake@gmail.com",
                Name = "fake name",
            };
            var fakeRequest = new Mock<HttpRequest>();
            string fakeInput = JsonConvert.SerializeObject(fakeUser);
            Stream content = new MemoryStream(Encoding.UTF8.GetBytes(fakeInput));
            fakeRequest.Setup(t => t.Body).Returns(content);

            _mockUserService.Setup(t => t.CreateUserAsync(fakeUser.Name,fakeUser.Email)).ReturnsAsync(It.IsAny<DomainUser>());

            //act
            var target = new UserApiFunctions(_mockUserService.Object);
            var result = await target.CreateUser(fakeRequest.Object);

            //asert
            Assert.Equal(200, ((OkObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task GivenUserDoesntExists_WhenModifyUser_ThenReturnsNotFound()
        {
            //arrange
            var fakeRequest = new Mock<HttpRequest>();
            UserUpdateRequest fakeUser = new UserUpdateRequest()
            {
                Email = "fake@gmail.com",
                Name = "fake name modified",
            };
            string fakeInput = JsonConvert.SerializeObject(fakeUser);
            Stream content = new MemoryStream(Encoding.UTF8.GetBytes(fakeInput));
            fakeRequest.Setup(t => t.Body).Returns(content);
            _mockUserService.Setup(t => t.GetUserByIdAsync(It.IsAny<string>()));

            //act
            var target = new UserApiFunctions(_mockUserService.Object);
            var result = await target.UpdateUser(fakeRequest.Object, It.IsAny<string>());

            //asert
            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Fact]
        public async Task GivenValidRequest_WhenModifyUser_ThenReturnsOK()
        {
            //arrange
            UserUpdateRequest fakeUser = new UserUpdateRequest()
            {
                Email = "fake@gmail.com",
                Name = "fake name modified",
            };
            var fakeRequest = new Mock<HttpRequest>();
            string fakeInput = JsonConvert.SerializeObject(fakeUser);
            Stream content = new MemoryStream(Encoding.UTF8.GetBytes(fakeInput));
            fakeRequest.Setup(t => t.Body).Returns(content);

            _mockUserService.Setup(t => t.GetUserByIdAsync(It.IsAny<string>())).ReturnsAsync(new DomainUser("old name", "fake@gmail.com"));
            _mockUserService.Setup(t => t.UpdateUserDetailsAsync(It.IsAny<string>(), fakeUser)).ReturnsAsync(It.IsAny<DomainUser>());

            //act
            var target = new UserApiFunctions(_mockUserService.Object);
            var result = await target.UpdateUser(fakeRequest.Object, It.IsAny<string>());

            //asert
            Assert.Equal(200, ((OkObjectResult)result).StatusCode);
        }

        [Fact]
        public async Task GivenUserDoesntExists_WhenDeleteUser_ThenReturnsNotFound()
        {
            //arrange
            var fakeRequest = new Mock<HttpRequest>();

            _mockUserService.Setup(t => t.DeleteUserAsync(It.IsAny<string>())).ReturnsAsync(false);

            //act
            var target = new UserApiFunctions(_mockUserService.Object);
            var result = await target.DeleteUser(fakeRequest.Object, It.IsAny<string>());

            //asert
            Assert.Equal(404, ((NotFoundResult)result).StatusCode);
        }

        [Fact]
        public async Task GivenValidRequest_WhenDeleteUser_ThenReturnsOK()
        {
            //arrange
            var fakeRequest = new Mock<HttpRequest>();
            UserCreationRequest fakeUser = new UserCreationRequest()
            {
                Email = "fake@gmail.com",
                Name = "fake name modified",
            };
            _mockUserService.Setup(t => t.DeleteUserAsync(It.IsAny<string>())).ReturnsAsync(true);

            //act
            var target = new UserApiFunctions(_mockUserService.Object);
            var result = await target.DeleteUser(fakeRequest.Object, It.IsAny<string>());

            //asert
            Assert.Equal(200, ((OkObjectResult)result).StatusCode);

        }
    }
}
