using System.ComponentModel.DataAnnotations;

namespace SharedModels.partonair_v01.DTOS
{
    public record SendMessageDTO(
        [Required]
        [StringLength(4000, MinimumLength = 1)]
        string Content);

    public record MessageViewDTO(
        Guid Id,
        Guid SenderId,
        Guid ReceiverId,
        string Content,
        DateTime SentAt,
        DateTime? ReadAt,
        string SenderName,
        string? SenderAvatarSeed);

    /// <summary>
    /// Historique d'une conversation. CanSend = false si un blocage existe ;
    /// BlockedBy vaut alors "me" (je l'ai bloqué) ou "other" (il m'a bloqué).
    /// </summary>
    public record ConversationViewDTO(ICollection<MessageViewDTO> Messages, bool CanSend, string? BlockedBy);

    /// <summary>Nombre de messages non lus reçus d'un contact.</summary>
    public record UnreadConversationDTO(Guid UserId, int Count);
}
