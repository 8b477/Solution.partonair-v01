

using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Enums;

namespace Domain.partonair_v01.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Mail { get; set; } = null!;
        public string PasswordHashed { get; set; } = null!;
        public Roles Role { get; set; } = Roles.Visitor;
        public DateTime UserCreatedAt { get; set; }
        public DateTime? UserUpdatedAt { get; set; }
        public DateTime LastConnection { get; set; }
        public bool IsActive { get; set; } = false;


        // Navigation properties
        public ProfileUser? ProfileUser { get; set; }
        public ICollection<Announcement> Announcements { get; set; } = [];
        public ICollection<Evaluation> EvaluationsReceived { get; set; } = [];
        public ICollection<Evaluation> EvaluationsSent { get; set; } = [];
        public ICollection<Contact> ContactsSent { get; set; } = [];
        public ICollection<Contact> ContactsReceived { get; set; } = [];
    }
}
