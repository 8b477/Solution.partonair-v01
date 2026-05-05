using Domain.partonair_v01.Entities;
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
                Skills = entity.Skills,
                UrlCv = entity.UrlCv,
                IsPublic = entity.IsPublic,
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
                e.UserId
            );
        }

        public static void ToEntity(this ProfileUpdateDTO p, ProfileUser e)
        {
            e.ProfileDescription = p.ProfileDescription;
            e.Skills = p.Skills;
            e.UrlCv = p.UrlCv;
            e.IsPublic = p.IsPublic;
        }
    }
}
