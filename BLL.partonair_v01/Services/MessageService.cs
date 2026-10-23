using SharedModels.partonair_v01.DTOS;

using BLL.partonair_v01.Mappers;
using BLL.partonair_v01.Interfaces;
using Domain.partonair_v01.Exceptions.Enums;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Entities;


namespace BLL.partonair_v01.Services
{
    public class MessageService(IUnitOfWork UOW, IContactService contactService) : IMessageService
    {
        private const int DefaultHistorySize = 50;
        private const int MaxHistorySize = 200;
        private const int MaxContentLength = 4000;

        private readonly IUnitOfWork _UOW = UOW;
        private readonly IContactService _contactService = contactService;


        public async Task<MessageViewDTO> SendAsyncService(Guid senderId, Guid receiverId, string content)
        {
            if (senderId == receiverId)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Tu ne peux pas t’envoyer un message à toi-même.");

            if (string.IsNullOrWhiteSpace(content))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Le message ne peut pas être vide.");

            var trimmed = content.Trim();
            if (trimmed.Length > MaxContentLength)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, $"Le message dépasse la longueur maximale autorisée ({MaxContentLength} caractères).");

            // Garde d'autorisation : seuls des amis réciproques non bloqués peuvent échanger des messages.
            if (!await _contactService.AreMutualFriendsAsyncService(senderId, receiverId))
            {
                var relationship = await _contactService.GetRelationshipAsyncService(senderId, receiverId);
                var reason = relationship.Status switch
                {
                    RelationshipStatus.BlockedByOther => "Ce contact t’a bloqué : tu ne peux plus lui écrire.",
                    RelationshipStatus.BlockedByMe => "Tu as bloqué ce contact. Débloque-le pour lui écrire.",
                    _ => "Tu ne peux envoyer un message qu’à un contact ami et non bloqué."
                };
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, reason);
            }

            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var sender = await _UOW.Users.GetByGuidAsync(senderId);
                var receiver = await _UOW.Users.GetByGuidAsync(receiverId);
                // Charge le profil de l'expéditeur (suivi par EF) pour exposer son avatar dans la notification.
                await _UOW.Profiles.GetByUserIdAsync(sender.Id);

                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    SenderId = sender.Id,
                    ReceiverId = receiver.Id,
                    Sender = sender,
                    Receiver = receiver,
                    Content = trimmed,
                    SentAt = DateTime.UtcNow
                };

                var created = await _UOW.Messages.CreateAsync(message);

                return created.ToView();
            });
        }

        public async Task<ConversationViewDTO> GetConversationAsyncService(Guid userId, Guid otherUserId, int take)
        {
            if (userId == otherUserId)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Aucune conversation avec toi-même.");

            // Amis : lecture + écriture. Blocage (dans un sens ou l'autre) : historique en lecture seule.
            var relationship = await _contactService.GetRelationshipAsyncService(userId, otherUserId);
            string? blockedBy = relationship.Status switch
            {
                RelationshipStatus.BlockedByMe => "me",
                RelationshipStatus.BlockedByOther => "other",
                _ => null
            };

            if (relationship.Status != RelationshipStatus.Friends && blockedBy is null)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Tu ne peux consulter que les conversations avec un contact ami.");

            var bounded = take <= 0 ? DefaultHistorySize : Math.Min(take, MaxHistorySize);

            var messages = await _UOW.Messages.GetConversationAsync(userId, otherUserId, bounded);

            return new ConversationViewDTO(
                messages.Select(message => message.ToView()).ToList(),
                CanSend: blockedBy is null,
                BlockedBy: blockedBy);
        }

        public async Task<ICollection<UnreadConversationDTO>> GetUnreadAsyncService(Guid userId)
        {
            var unread = await _UOW.Messages.CountUnreadBySenderAsync(userId);

            return unread
                .Select(entry => new UnreadConversationDTO(entry.Key, entry.Value))
                .ToList();
        }

        public async Task MarkConversationReadAsyncService(Guid userId, Guid otherUserId)
        {
            // Ne touche qu'aux messages reçus par l'utilisateur courant : aucune garde d'amitié nécessaire.
            await _UOW.Messages.MarkConversationReadAsync(userId, otherUserId);
        }
    }
}
