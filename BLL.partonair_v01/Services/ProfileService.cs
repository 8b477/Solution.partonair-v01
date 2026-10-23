using BLL.partonair_v01.Interfaces;
using BLL.partonair_v01.Mappers;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;
using SharedModels.partonair_v01.DTOS;

namespace BLL.partonair_v01.Services
{
    public class ProfileService(IUnitOfWork UOW, IContactService contactService) : IProfileService
    {
        private const int MaxAvatarRegenPerDay = 10;

        private readonly IUnitOfWork _UOW = UOW;
        private readonly IContactService _contactService = contactService;

        public async Task<ProfileViewDTO> CreateAsyncService(Guid idUser, ProfileCreateDTO profileDTO)
        {
            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var existingUser = await _UOW.Users.GetByGuidAsync(idUser);

                if (existingUser.ProfileUser is not null)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException,
                        "The user already has a profile, please use Update endpoint or Delete before creating a new profile.");

                var allowedRoles = new[] { Roles.Visitor, Roles.Employee, Roles.Company };

                if (!allowedRoles.Contains(profileDTO.Role))
                {
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException,
                        "You are not allowed to assign this role.");
                }

                existingUser.Role = profileDTO.Role;

                EnsureRoleRequirements(profileDTO.Role, profileDTO.Skills, profileDTO.RecruiterNeeds);

                ProfileUser profileToAdd = profileDTO.ToEntity(existingUser);
                profileToAdd.ApplyRecruiterNeeds(profileDTO.RecruiterNeeds, profileDTO.Role);

                var profilCreated = await _UOW.Profiles.CreateAsync(profileToAdd);

                existingUser.ProfileUser = profilCreated;

                var result = await _UOW.Users.Update(existingUser);

                return profilCreated.ToView();
            });
        }

        public async Task<ProfileViewDTO> GetByGuidAsyncService(Guid id)
        {
            var result = await _UOW.Profiles.GetByGuidAsync(id);

            return result.ToView();
        }

        /// <summary>
        /// Fiche d'un membre. Un profil privé reste visible par son propriétaire, ses amis
        /// et les membres qui lui ont envoyé une demande d'ami (pour décider avant d'accepter).
        /// </summary>
        public async Task<PublicProfileViewDTO?> GetPublicByGuidAsyncService(Guid id, Guid? viewerId)
        {
            var profile = await _UOW.Profiles.GetWithUserByGuidAsync(id);

            if (profile is null)
                return null;

            if (!profile.IsPublic && !await CanSeePrivateProfileAsync(viewerId, profile.UserId))
                return null;

            return profile.ToPublicView(profile.User);
        }

        private async Task<bool> CanSeePrivateProfileAsync(Guid? viewerId, Guid ownerId)
        {
            if (viewerId is null)
                return false;

            if (viewerId == ownerId)
                return true;

            var relationship = await _contactService.GetRelationshipAsyncService(viewerId.Value, ownerId);

            return relationship.Status is RelationshipStatus.Friends or RelationshipStatus.PendingIncoming;
        }

        public async Task<ICollection<PublicProfileViewDTO>> GetByRoleAsyncService(string role, Guid? currentUserId)
        {
            if (!Enum.TryParse<Roles>(role, true, out var validRole))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Les rôles autorisés sont Employee et Company.");

            if (validRole is not Roles.Employee and not Roles.Company)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Les rôles autorisés sont Employee et Company.");

            var users = await _UOW.Users.GetByRoleIncludeProfilAsync(validRole.ToString());

            // Seuls les profils publics sont exposés dans l'annuaire ; l'utilisateur courant peut masquer sa propre carte.
            var publicUsers = users
                .Where(u => u.ProfileUser is not null && u.ProfileUser.IsPublic)
                .Where(u => u.Id != currentUserId || u.ProfileUser!.ShowSelfInDirectory)
                .ToList();
            ICollection<PublicProfileViewDTO> finalView = publicUsers
                .Select(u => u.ProfileUser!.ToPublicView(u))
                .ToList();

            return finalView;
        }

        public async Task<ProfileViewDTO> UpdateAsyncService(Guid idUser, Guid id, ProfileUpdateDTO entity)
        {
            var existingProfil = await _UOW.Profiles.GetByGuidAsync(id);

            if (existingProfil.UserId != idUser)
                throw new ApplicationLayerException(ApplicationLayerErrorType.EntityIsNotExistingException, "Ce profil est introuvable ou ne t’appartient pas.");

            entity.ToEntity(existingProfil);
            existingProfil.ProfileUpdatedAt = DateTime.Now;

            var result = await _UOW.Profiles.Update(existingProfil);

            await _UOW.SaveChangesAsync();

            return result.ToView();
        }

        public async Task<MyProfileViewDTO> GetMyProfileAsyncService(Guid idUser)
        {
            var user = await GetUserWithProfileAsync(idUser);

            return user.ProfileUser!.ToMyView(user, AvatarRegenRemaining(user.ProfileUser!));
        }

        public async Task<MyProfileViewDTO> UpdateMyProfileAsyncService(Guid idUser, MyProfileUpdateDTO entity)
        {
            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var user = await GetUserWithProfileAsync(idUser);
                var profile = user.ProfileUser!;
                var newName = entity.Name.Trim();

                if (newName != user.Name)
                {
                    user.Name = newName;
                    user.UserUpdatedAt = DateTime.Now;

                    // ContactName est dénormalisé (nom du destinataire) : on le garde synchronisé.
                    var contacts = await _UOW.Contacts.GetAllByReceiverAsync(user.Id);
                    foreach (var contact in contacts)
                        contact.ContactName = newName;
                }

                EnsureRoleRequirements(user.Role, entity.Skills, entity.RecruiterNeeds);

                entity.ToEntity(profile);
                profile.ApplyRecruiterNeeds(entity.RecruiterNeeds, user.Role);
                profile.ProfileUpdatedAt = DateTime.Now;

                return profile.ToMyView(user, AvatarRegenRemaining(profile));
            });
        }

        public async Task<AvatarRegenerateResultDTO> RegenerateAvatarAsyncService(Guid idUser)
        {
            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var user = await GetUserWithProfileAsync(idUser);
                var profile = user.ProfileUser!;
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                if (profile.AvatarRegenDate != today)
                {
                    profile.AvatarRegenDate = today;
                    profile.AvatarRegenCount = 0;
                }

                // Quota côté serveur pour ne pas spammer l'API DiceBear.
                if (profile.AvatarRegenCount >= MaxAvatarRegenPerDay)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.QuotaExceededException,
                        $"Tu as atteint la limite de {MaxAvatarRegenPerDay} nouveaux avatars pour aujourd’hui. Réessaie demain.");

                profile.AvatarSeed = ProfileMapper.NewAvatarSeed();
                profile.AvatarRegenCount++;

                return new AvatarRegenerateResultDTO(profile.AvatarSeed, AvatarRegenRemaining(profile));
            });
        }

        public async Task DeleteAsyncService(Guid idUser, Guid idProfile)
        {
            var existingUser = await _UOW.Users.GetByForeignKeyProfilAsync(idUser);
                
            if(existingUser is not null && existingUser?.ProfileUser?.Id == idProfile)
            {
                await _UOW.Profiles.Delete(idProfile);

                await _UOW.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"L'utilisateur : {idUser} n'est pas lié avec l'id du profile fournis :'{idProfile}'.");
            }

            return;
        }

        // Lève une 404 (ResourceNotFound) si l'utilisateur n'a pas encore de profil.
        private Task<User> GetUserWithProfileAsync(Guid idUser) => _UOW.Users.GetByForeignKeyProfilAsync(idUser);

        // Chercheur d'emploi : compétences obligatoires. Recruteur : poste recherché obligatoire, compétences optionnelles.
        private static void EnsureRoleRequirements(Roles role, string? skills, RecruiterNeedsDTO? recruiterNeeds)
        {
            if (role == Roles.Employee && string.IsNullOrWhiteSpace(skills))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Renseigne au moins une compétence.");

            if (role == Roles.Company && string.IsNullOrWhiteSpace(recruiterNeeds?.SoughtPosition))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Indique le poste ou le profil que tu recherches.");
        }

        private static int AvatarRegenRemaining(ProfileUser profile)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var usedToday = profile.AvatarRegenDate == today ? profile.AvatarRegenCount : 0;

            return Math.Max(0, MaxAvatarRegenPerDay - usedToday);
        }
    }
}
