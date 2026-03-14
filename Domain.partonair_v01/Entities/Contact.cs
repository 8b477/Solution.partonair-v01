using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Enums;
namespace Domain.partonair_v01.Entities
{
    public class Contact : IEntity
    {
        public Guid Id { get; set; }
        public string ContactName { get; set; } = null!;
        public string ContactMail { get; set; } = null!;
        public bool? IsFriendly { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? BlockedAt { get; set; }
        public bool? IsBlocked { get; set; }
        public StatusContact ContactStatus { get; set; } = StatusContact.Pending;


        // FK
        public Guid ContactReceiverId { get; set; }
        public Guid ContactSenderId { get; set; }

        // Navigation properties
        public User ContactReceiver { get; set; } = null!;
        public User ContactSender { get; set; } = null!;
    }
}
