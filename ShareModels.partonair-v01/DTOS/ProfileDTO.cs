using System.ComponentModel.DataAnnotations;


namespace SharedModels.partonair_v01.DTOS
{
    public record ProfileCreateDTO
        (
            [Required(ErrorMessage = "The field is required")]
            [MinLength(20,ErrorMessage = "The field must contain at least 20 characters")]
            string ProfileDescription
        );

    public record ProfileViewDTO(Guid Id, string ProfilDescritpion, Guid FK_User);
    public record ProfileUpdateDTO
        (
            [Required(ErrorMessage = "The field is required")]
            [MinLength(20,ErrorMessage = "The field must contain at least 20 characters")]
            string ProfilDescritpion
        );
    public record ProfileAndUserViewDTO(UserViewDTO User, ProfileViewDTO Profile);
}
