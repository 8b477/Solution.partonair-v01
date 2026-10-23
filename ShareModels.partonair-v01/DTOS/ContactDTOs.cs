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
         [Required(ErrorMessage = "The field ID_Receiver is required")]
         Guid Id_Receiver
    );

    public record ContactRequestResultDTO(Guid ContactId, string Status);

    public record MyContactViewDTO(
        Guid ContactId,
        Guid UserId,
        string UserName,
        string Role,
        string Status,
        DateTime? AcceptedAt,
        DateTime? BlockedAt,
        string? AvatarSeed,
        Guid? ProfileId);

    /// <summary>Valeurs possibles de <see cref="RelationshipDTO.Status"/> (point de vue de l'utilisateur courant).</summary>
    public static class RelationshipStatus
    {
        public const string None = "None";
        public const string Friends = "Friends";
        public const string PendingOutgoing = "PendingOutgoing";
        public const string PendingIncoming = "PendingIncoming";
        public const string Refused = "Refused";
        public const string BlockedByMe = "BlockedByMe";
        public const string BlockedByOther = "BlockedByOther";
    }

    /// <summary>Relation entre l'utilisateur courant et un autre utilisateur. ContactId = demande en attente à accepter, le cas échéant.</summary>
    public record RelationshipDTO(string Status, Guid? ContactId);

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
