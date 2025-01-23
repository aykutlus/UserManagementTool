using ManagementTool.Functions.Domain;
using ManagementTool.Functions.Infrastructure;
using ManagementTool.Functions.Presentation.Models;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<List<DomainUser>> GetUsersAsync(bool includeDeleted)
    {
        var users = await _userRepository.GetAllAsync();

        if (!includeDeleted)
        {
            users = users.Where(user =>
                user.History.All(entry => entry.ChangeType != ChangeType.Deleted)
            ).ToList();
        }

        return users;
    }

    public async Task<DomainUser> GetUserByIdAsync(string id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<DomainUser> CreateUserAsync(string name, string email)
    {
        var user = new DomainUser(name, email);
        await _userRepository.AddAsync(user);
        return user;
    }

    public async Task<DomainUser> UpdateUserDetailsAsync(string id, UserUpdateRequest data)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new Exception($"User with ID {id} not found.");

        if (!string.IsNullOrEmpty(data.Name) || !string.IsNullOrEmpty(data.Email))
        {
            var oldDetails = new { Name = user.Name, Email = user.Email };
            user.UpdateDetails(data.Name ?? user.Name, data.Email ?? user.Email);
            user.AddHistoryEntry(ChangeType.ModifiedUserDetails,oldDetails,new { Name = user.Name, Email = user.Email });
        }

        if (data.Credits.HasValue && data.Credits.Value != user.Credits)
        {
            var oldCredits = user.Credits;
            user.AddCredits(data.Credits.Value - oldCredits);
            user.AddHistoryEntry(ChangeType.ModifiedUserCredits,oldCredits,data.Credits.Value);
        }

        await _userRepository.UpdateAsync(user);

        return user;
    }


    public async Task<bool> DeleteUserAsync(string id)
    {
        return await _userRepository.DeleteAsync(id);
    }
}
