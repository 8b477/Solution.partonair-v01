

using Domain.partonair_v01.Contracts;

namespace Domain.partonair_v01.Entities
{
    public class Announcement : IEntity
    {
        public Guid Id { get; set; }
        public string JobTag { get; set; } = null!;
        public string SkillsRequired { get; set; } = null!;
        public string AnnouncementDescription { get; set; } = null!;
        public DateTime AnnouncementCreatedAt { get; set; }
        public DateTime? AnnouncementUpdatedAt { get; set; }


        // FK
        public Guid UserId { get; set; }

        // Navigation prooperties
        public User User { get; set; } = null!;
    }
}
