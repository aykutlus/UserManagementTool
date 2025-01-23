using ManagementTool.Functions.Domain;

namespace ManagementTool.Functions.Infrastructure
{
    public interface IUserRepository
    {
        Task<List<DomainUser>> GetAllAsync();
        Task<DomainUser> GetByIdAsync(string id);
        Task UpdateAsync(DomainUser domainUser);
        Task AddAsync(DomainUser user);
        Task<bool> DeleteAsync(string id);
    }

}