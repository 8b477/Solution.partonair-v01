
using Domain.partonair_v01.Contracts;

namespace Domain.partonair_v01.Entities
{
    public class Images : IEntity
    {
        public Guid Id { get; set; }
        public byte[]? ProfileImageBits { get; set; }
        public string? TypeProfileImage { get; set; }
        public int? SizeProfileImage { get; set; }
        public byte[]? CvImageBits { get; set; }
        public string? TypeCvImage { get; set; }
        public int? SizeCvImage { get; set; }

        // FK
        public Guid FkProfileUser { get; set; }

        // Navigation properties
        public ProfileUser ProfileUser { get; set; } = null!;
    }
}
