using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Mappers
{
    public static class ProfileMapper
    {
        public static ProfileUser ToEntity(this ProfileCreateDTO entity, User u)
        {
            return new ProfileUser
            {
                Id = Guid.NewGuid(),
                ProfileDescription = entity.ProfileDescription,
                Skills = entity.Skills?.Trim() ?? string.Empty,
                UrlCv = entity.UrlCv,
                IsPublic = entity.IsPublic,
                ShowSelfInDirectory = true,
                AvatarSeed = NewAvatarSeed(),
                ProfileCreatedAt = DateTime.Now,
                UserId = u.Id,
                User = u
            };
        }

        public static ProfileViewDTO ToView(this ProfileUser e)
        {
            return new ProfileViewDTO(
                e.Id,
                e.ProfileDescription,
                e.Skills,
                e.UrlCv,
                e.IsPublic,
                e.Stars,
                e.ProfileCreatedAt,
                e.ProfileUpdatedAt,
                e.UserId,
                e.ShowSelfInDirectory,
                e.AvatarSeed,
                e.ToRecruiterNeeds(e.User?.Role)
            );
        }

        public static PublicProfileViewDTO ToPublicView(this ProfileUser e, User u)
        {
            return new PublicProfileViewDTO(
                e.Id,
                u.Id,
                u.Name,
                u.Role.ToString(),
                e.ProfileDescription,
                e.Skills,
                e.UrlCv,
                e.Stars,
                e.AvatarSeed,
                e.ToRecruiterNeeds(u.Role),
                e.RatingAverage,
                e.RatingCount);
        }

        public static MyProfileViewDTO ToMyView(this ProfileUser e, User u, int avatarRegenRemaining)
        {
            return new MyProfileViewDTO(
                e.Id,
                u.Id,
                u.Name,
                u.Role.ToString(),
                e.ProfileDescription,
                e.Skills,
                e.UrlCv,
                e.IsPublic,
                e.ShowSelfInDirectory,
                e.Stars,
                e.AvatarSeed,
                avatarRegenRemaining,
                e.ProfileCreatedAt,
                e.ProfileUpdatedAt,
                e.ToRecruiterNeeds(u.Role),
                e.RatingAverage,
                e.RatingCount);
        }

        public static void ToEntity(this ProfileUpdateDTO p, ProfileUser e)
        {
            e.ProfileDescription = p.ProfileDescription;
            e.Skills = p.Skills;
            e.UrlCv = p.UrlCv;
            e.IsPublic = p.IsPublic;
            e.ShowSelfInDirectory = p.ShowSelfInDirectory;
        }

        public static void ToEntity(this MyProfileUpdateDTO p, ProfileUser e)
        {
            e.ProfileDescription = p.ProfileDescription;
            e.Skills = p.Skills?.Trim() ?? string.Empty;
            e.UrlCv = string.IsNullOrWhiteSpace(p.UrlCv) ? null : p.UrlCv;
            e.IsPublic = p.IsPublic;
            e.ShowSelfInDirectory = p.ShowSelfInDirectory;
        }

        public static string NewAvatarSeed() => Guid.NewGuid().ToString("N");

        /// <summary>Applique les besoins recruteur, ou les efface si le profil n'est pas un recruteur.</summary>
        public static void ApplyRecruiterNeeds(this ProfileUser e, RecruiterNeedsDTO? needs, Roles role)
        {
            var keep = role == Roles.Company ? needs : null;

            e.SoughtPosition = string.IsNullOrWhiteSpace(keep?.SoughtPosition) ? null : keep.SoughtPosition.Trim();
            e.ContractTypes = keep?.ContractTypes?.Aggregate(ContractTypes.None, (all, type) => all | type) ?? ContractTypes.None;
            e.Location = string.IsNullOrWhiteSpace(keep?.Location) ? null : keep.Location.Trim();
            e.RemotePolicy = keep?.RemotePolicy;
            e.Sector = keep?.Sector;
        }

        public static RecruiterNeedsDTO? ToRecruiterNeeds(this ProfileUser e, Roles? role)
        {
            if (role is not Roles.Company)
                return null;

            var contractTypes = Enum.GetValues<ContractTypes>()
                .Where(type => type != ContractTypes.None && e.ContractTypes.HasFlag(type))
                .ToList();

            return new RecruiterNeedsDTO(e.SoughtPosition, contractTypes, e.Location, e.RemotePolicy, e.Sector);
        }
    }
}
