using Domain.partonair_v01.Enums;
using SharedModels.partonair_v01.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SharedModels.partonair_v01.DTOS
{
    public record ProfileCreateDTO
        (
            [Required(ErrorMessage = "The field is required")]
            [MinLength(20,ErrorMessage = "The field must contain at least 20 characters")]
            string ProfileDescription,

            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string? Skills,

            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string? UrlCv = null,

            bool IsPublic = false,

            [EnumDataType(typeof(Roles), ErrorMessage = "Invalid role.")]
            [AllowedRolesAttribute(Roles.Visitor, Roles.Employee, Roles.Company)]
            Roles Role = Roles.Visitor,

            RecruiterNeedsDTO? RecruiterNeeds = null
        );

    /// <summary>Ce que recherche un recruteur. Ignoré pour les autres rôles.</summary>
    public record RecruiterNeedsDTO
        (
            [MaxLength(150, ErrorMessage = "The field must not exceed 150 characters")]
            string? SoughtPosition,

            ICollection<ContractTypes>? ContractTypes,

            [MaxLength(100, ErrorMessage = "The field must not exceed 100 characters")]
            string? Location,

            RemotePolicy? RemotePolicy,

            Sector? Sector
        );

    public record ProfileViewDTO(
        Guid Id,
        string ProfileDescription,
        string Skills,
        string? UrlCv,
        bool IsPublic,
        int Stars,
        DateTime ProfileCreatedAt,
        DateTime? ProfileUpdatedAt,
        Guid FK_User,
        bool ShowSelfInDirectory,
        string AvatarSeed,
        RecruiterNeedsDTO? RecruiterNeeds
    );

    public record ProfileUpdateDTO
        (
            [Required(ErrorMessage = "The field is required")]
            [MinLength(20,ErrorMessage = "The field must contain at least 20 characters")]
            string ProfileDescription,

            [Required(ErrorMessage = "The field is required")]
            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string Skills,

            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string? UrlCv = null,

            bool IsPublic = false,

            bool ShowSelfInDirectory = true
        );

    public record ProfileAndUserViewDTO(UserViewDTO User, ProfileViewDTO? Profile);
    public record PublicProfileViewDTO(
        Guid ProfileId,
        Guid UserId,
        string UserName,
        string Role,
        string ProfileDescription,
        string Skills,
        string? UrlCv,
        int Stars,
        string AvatarSeed,
        RecruiterNeedsDTO? RecruiterNeeds,
        decimal RatingAverage,
        int RatingCount);

    /// <summary>Vue complète du profil de l'utilisateur connecté (page « Mon profil »).</summary>
    public record MyProfileViewDTO(
        Guid ProfileId,
        Guid UserId,
        string UserName,
        string Role,
        string ProfileDescription,
        string Skills,
        string? UrlCv,
        bool IsPublic,
        bool ShowSelfInDirectory,
        int Stars,
        string AvatarSeed,
        int AvatarRegenRemaining,
        DateTime ProfileCreatedAt,
        DateTime? ProfileUpdatedAt,
        RecruiterNeedsDTO? RecruiterNeeds,
        decimal RatingAverage,
        int RatingCount);

    /// <summary>Mise à jour du profil de l'utilisateur connecté, nom d'affichage compris.</summary>
    public record MyProfileUpdateDTO
        (
            [Required(ErrorMessage = "The field is required")]
            [MinLength(2, ErrorMessage = "The field must contain at least 2 characters")]
            [MaxLength(200, ErrorMessage = "The field must not exceed 200 characters")]
            string Name,

            [Required(ErrorMessage = "The field is required")]
            [MinLength(20,ErrorMessage = "The field must contain at least 20 characters")]
            [MaxLength(1000, ErrorMessage = "The field must not exceed 1000 characters")]
            string ProfileDescription,

            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string? Skills,

            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string? UrlCv,

            bool IsPublic,

            bool ShowSelfInDirectory,

            RecruiterNeedsDTO? RecruiterNeeds = null
        );

    public record AvatarRegenerateResultDTO(string AvatarSeed, int Remaining);
}
