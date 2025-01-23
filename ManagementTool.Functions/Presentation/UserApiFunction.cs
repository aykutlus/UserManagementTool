using System.Text.Json;
using ManagementTool.Functions.Domain;
using ManagementTool.Functions.Presentation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace ManagementTool.Functions.Presentation
{
    public class UserApiFunctions
    {
        private readonly IUserService _userService;

        public UserApiFunctions(IUserService userService)
        {
            _userService = userService;
        }

        [Function("ListUsers")]
        public async Task<IActionResult> ListUsers(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users")] HttpRequest req)
        {
            var includeDeleted = req.Query.ContainsKey("deleted") && bool.TryParse(req.Query["deleted"], out bool include) && include;
            var users = await _userService.GetUsersAsync(includeDeleted);
            return new OkObjectResult(users);
        }

        [Function("GetUserById")]
        public async Task<IActionResult> GetUserById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id}")] HttpRequest req, string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return new NotFoundResult();
            return new OkObjectResult(user);
        }

        [Function("CreateUser")]
        public async Task<IActionResult> CreateUser(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users")] HttpRequest req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<UserCreationRequest>(requestBody);

            if (data == null || string.IsNullOrEmpty(data.Name) || string.IsNullOrEmpty(data.Email))
                return new BadRequestObjectResult("Invalid input");

            var user = await _userService.CreateUserAsync(data.Name, data.Email);
            return new OkObjectResult(user);
        }

        [Function("UpdateUser")]
        public async Task<IActionResult> UpdateUser(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "users/{id}")] HttpRequest req, string id)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonSerializer.Deserialize<UserUpdateRequest>(requestBody);

            if (data == null)
                return new BadRequestObjectResult("Invalid input");

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return new NotFoundResult();

            var updatedUser = await _userService.UpdateUserDetailsAsync(id, data);

            return new OkObjectResult(updatedUser);
        }

        [Function("DeleteUser")]
        public async Task<IActionResult> DeleteUser(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{id}")] HttpRequest req, string id)
        {
            var success = await _userService.DeleteUserAsync(id);
            if (!success) return new NotFoundResult();
            return new OkObjectResult($"User with ID {id} deleted.");
        }
    }
}
