using System.ComponentModel.DataAnnotations;


namespace SharedModels.partonair_v01.DTOS
{
    public record ProfileCreateDTO
        (
            [Required(ErrorMessage = "The field is required")]
            [MinLength(20,ErrorMessage = "The field must contain at least 20 characters")]
            string ProfileDescription,

            [Required(ErrorMessage = "The field is required")]
            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string Skills,

            [MaxLength(500, ErrorMessage = "The field must not exceed 500 characters")]
            string? UrlCv = null,

            bool IsPublic = false
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
        Guid FK_User
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

            bool IsPublic = false
        );

    public record ProfileAndUserViewDTO(UserViewDTO User, ProfileViewDTO? Profile);
}
