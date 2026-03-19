using System.ComponentModel.DataAnnotations;


namespace SharedModels.partonair_v01.DTOS
{

    public record ContactViewDTO
    (
        Guid Id,
        Guid Id_Sender,
        Guid Id_Receiver,
        string ContactName,
        string ContactEmail,
        bool? IsFriendly,
        bool? IsBlocked,
        DateTime? BlockedAt,
        DateTime? AcceptedAt,
        string ContactStatus
    );

    public record ContactCreateDTO
    (
         [Required(ErrorMessage = "The field ID_Sender is required")]
         Guid Id_Sender,

         [Required(ErrorMessage = "The field ID_Receiver is required")]
         Guid Id_Receiver
    );

    public record UserToLock
    (
        [Required(ErrorMessage = "The field is required")]
        Guid Id_UserToLock
    );

    public record UserToUnlock
    (
        [Required(ErrorMessage = "The field is required")]
        Guid Id_UserToUnlock
    );
}
