using Domain.partonair_v01.Contracts;

namespace Domain.partonair_v01.Entities
{
    public class ProfileUser : IEntity
    {
        public Guid Id { get; set; }
        public string ProfileDescription { get; set; } = null!;
        public string Skills { get; set; } = null!;
        public string? UrlCv { get; set; }
        public bool IsPublic { get; set; } = false;
        public int Stars { get; set; }
        public DateTime ProfileCreatedAt { get; set; }  
        public DateTime? ProfileUpdatedAt { get; set; }


        // FK
        public Guid UserId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Images? Image { get; set; }
    }
}