
namespace ManagementTool.Functions.Domain
{
    public class DomainUser
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public int Credits { get; private set; }
        private List<UserHistoryEntry> _history = new();

        public IReadOnlyCollection<UserHistoryEntry> History => _history.AsReadOnly();

        public DomainUser(string name, string email)
        {
            Id = Guid.NewGuid().ToString();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        public static DomainUser CreateWithIdAndCredits(string id, string name, string email, int credits)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("ID cannot be null or empty");
            var user = new DomainUser(name, email)
            {
                Id = id,
                Credits = credits
            };
            return user;
        }

        public void UpdateDetails(string newName, string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("Invalid user details");

            var oldDetails = new { Name, Email };
            Name = newName;
            Email = newEmail;

            AddHistoryEntry(ChangeType.ModifiedUserDetails, oldDetails, new { Name, Email });
        }

        public void AddCredits(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");

            var oldCredits = Credits;
            Credits += amount;
            AddHistoryEntry(ChangeType.ModifiedUserCredits, oldCredits, Credits);
        }

        public void AddHistoryEntry(ChangeType changeType, object oldValue, object newValue)
        {
            _history.Add(new UserHistoryEntry(changeType, DateTime.UtcNow, "System", oldValue, newValue));
        }
    }
}
