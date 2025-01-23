using ManagementTool.Functions.Domain;

namespace ManagementTool.Functions.Infrastructure
{
    public class CosmosUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Credits { get; set; }
        public List<UserHistoryEntry> History { get; set; } = new();

        public CosmosUser() { }

        public CosmosUser(DomainUser domainUser)
        {
            Id = domainUser.Id;
            Name = domainUser.Name;
            Email = domainUser.Email;
            Credits = domainUser.Credits;
            History = domainUser.History.ToList();
        }

        public DomainUser ToDomain()
        {
            // Create a DomainUser instance using the factory method
            var domainUser = DomainUser.CreateWithIdAndCredits(Id, Name, Email, Credits);

            foreach (var entry in History)
            {
                domainUser.AddHistoryEntry(entry.ChangeType,entry.OldValue,entry.NewValue);
            }

            return domainUser;
        }
    }
}
