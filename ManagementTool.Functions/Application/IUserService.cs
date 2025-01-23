using ManagementTool.Functions.Domain;
using ManagementTool.Functions.Presentation.Models;

public interface IUserService
{
    Task<DomainUser> CreateUserAsync(string name, string email);
    Task<DomainUser> GetUserByIdAsync(string id);
    Task<List<DomainUser>> GetUsersAsync(bool includeDeleted);
    Task<DomainUser> UpdateUserDetailsAsync(string id, UserUpdateRequest data);
    Task<bool> DeleteUserAsync(string id);
}