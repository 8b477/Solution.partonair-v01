using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Enums;

namespace Domain.partonair_v01.Entities
{
    public class ProfileUser : IEntity
    {
        public Guid Id { get; set; }
        public string ProfileDescription { get; set; } = null!;
        public string Skills { get; set; } = null!;
        public string? UrlCv { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool ShowSelfInDirectory { get; set; } = true;
        public string AvatarSeed { get; set; } = null!;
        public int AvatarRegenCount { get; set; }
        public DateOnly? AvatarRegenDate { get; set; }

        // Besoins du recruteur (renseignés uniquement si Role == Company)
        public string? SoughtPosition { get; set; }
        public ContractTypes ContractTypes { get; set; } = ContractTypes.None;
        public string? Location { get; set; }
        public RemotePolicy? RemotePolicy { get; set; }
        public Sector? Sector { get; set; }
        public int Stars { get; set; }
        public decimal RatingAverage { get; set; }
        public int RatingCount { get; set; }
        public DateTime ProfileCreatedAt { get; set; }  
        public DateTime? ProfileUpdatedAt { get; set; }


        // FK
        public Guid UserId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Images? Image { get; set; }
    }
}