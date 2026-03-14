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
                ProfileDescription = entity.ProfileDescription,
                UserId = u.Id,
                User = u
            };
        }

        public static ProfileViewDTO ToView(this ProfileUser e)
        {
            return new ProfileViewDTO(e.Id,e.ProfileDescription,e.UserId);
        } 

        public static void ToEntity(this ProfileUpdateDTO p, ProfileUser e)
        {
            e.ProfileDescription = p.ProfilDescritpion;            
        }
    }
}
