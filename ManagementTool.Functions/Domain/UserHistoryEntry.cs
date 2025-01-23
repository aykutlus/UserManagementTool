using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementTool.Functions.Domain
{
    public class UserHistoryEntry
    {
        public ChangeType ChangeType { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string ChangedBy { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public UserHistoryEntry(ChangeType changeType, DateTime timestamp, string changedBy, object oldValue, object newValue)
        {
            ChangeType = changeType;
            Timestamp = timestamp;
            ChangedBy = changedBy ?? throw new ArgumentNullException(nameof(changedBy));
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}

