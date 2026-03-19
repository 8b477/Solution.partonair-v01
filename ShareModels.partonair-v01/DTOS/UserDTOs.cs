using System.ComponentModel.DataAnnotations;

namespace SharedModels.partonair_v01.DTOS
{
    public record UserViewDTO
    (
        Guid Id,
        string UserName,
        string Email,
        bool IsPublic,
        DateTime UserCreatedAt,
        DateTime LastConnection,
        string Role,
        Guid? FK_Profile
    );

    public class UserCreateDTO
    {
        [Required(ErrorMessage = "Le champ 'Nom' est requis")]
        [MinLength(3, ErrorMessage = "Le champ 'Nom' doit contenir au minimum 3 caractères")]
        [MaxLength(200, ErrorMessage = "Le champ 'Nom' doit contenir au maximum 200 caractères")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Le champ 'Email' est requis")]
        [EmailAddress(ErrorMessage = "Adresse email non valide")]
        [MaxLength(250, ErrorMessage = "Le champ 'Email' doit contenir au maximum 250 caractères")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Le champ 'Mot de passe' est requis")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Le mot de passe doit avoir au moins 8 caractères avec 1 majuscule, 1 minuscule, 1 chiffre et 1 caractère spécial")]
        public string Password { get; set; } = null!;


        [Required(ErrorMessage = "Le champ ne correspond pas avec le 'Mot de passe'")]
        [Compare(nameof(Password), ErrorMessage = "Le champ ne correspond pas avec le 'Mot de passe'")]
        public string ConfirmPassword { get; set; } = null!;
    };

    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Le champ 'Email' est requis")]
        [EmailAddress(ErrorMessage = "Adresse email non valide")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Le champ 'Mot de passe' est requis")]
        public string Password { get; set; } = null!;
    };

    public class UserUpdateNameOrMailOrPasswordDTO
    {
        [MinLength(3,ErrorMessage = "Le champ 'Nom' doit contenir au minimum 3 caractères")]
        [MaxLength(200,ErrorMessage = "Le champ 'Nom' doit contenir au maximum 200 caractères")]
        public string? UserName { get; set; }

        [EmailAddress(ErrorMessage = "Adresse email non valide")]
        [MaxLength(250,ErrorMessage = "Le champ 'Email' doit contenir au maximum 250 caractères")]
        public string? Email { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Le mot de passe doit avoir au moins 8 caractères avec 1 majuscule, 1 minuscule, 1 chiffre et 1 caractère spécial")]
        public string? OldPassword { get; set; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", ErrorMessage = "Le mot de passe doit avoir au moins 8 caractères avec 1 majuscule, 1 minuscule, 1 chiffre et 1 caractère spécial")]
        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "Le nouveau mot de passe et la confirmation de celui-ci ne corresponde pas")]
        public string? NewPasswordConfirm { get; set; }
    };

    //public record UserChangeRoleDTO
    //(
    //    [Required]
    //    [ValidRole]      
    //    string Role
    //);

}
