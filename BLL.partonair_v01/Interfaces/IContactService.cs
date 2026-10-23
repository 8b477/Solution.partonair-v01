using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Interfaces
{
    public interface IContactService
    {
        Task<ContactRequestResultDTO> CreateAsyncService(Guid senderId, Guid receiverId);
        Task DeleteAsyncService(Guid idSender, Guid idContact);

        Task<ICollection<ContactViewDTO>> GetAllAsyncService();
        Task<ICollection<ContactViewDTO>> GetAllPendingStatusAsyncService(string status);
        Task<ICollection<ContactViewDTO>> GetAllAcceptedStatusAsyncService(string status);
        Task<ICollection<ContactViewDTO>> GetAllBlockedStatusAsyncService(string status);
        Task<ContactViewDTO> GetByGuidAsyncService(Guid id);
        Task<ICollection<ContactViewDTO>> GetByNameAsyncService(string name);
        Task<ContactViewDTO> GetByEmailAsyncService(string email);

        Task<ICollection<MyContactViewDTO>> GetFriendsAsyncService(Guid userId);
        Task<ICollection<MyContactViewDTO>> GetIncomingRequestsAsyncService(Guid userId);
        Task<ICollection<MyContactViewDTO>> GetOutgoingRequestsAsyncService(Guid userId);
        Task<ICollection<MyContactViewDTO>> GetBlockedContactsAsyncService(Guid userId);
        Task<ICollection<MyContactViewDTO>> GetConversationContactsAsyncService(Guid userId);
        Task<RelationshipDTO> GetRelationshipAsyncService(Guid userId, Guid otherUserId);

        Task<string> AcceptedRequestAsyncService(Guid userId, Guid idContact);
        Task<string> RefusedRequestAsyncService(Guid userId, Guid idContact);
        Task<string> LockContactRequestAsyncService(Guid idSender, UserToLock UserToLock);
        Task<string> UnlockContactRequestAsyncService(Guid idSender, UserToUnlock UserToUnlock);

        /// <summary>
        /// Garde d'autorisation réutilisable : vrai uniquement si les deux utilisateurs sont amis
        /// réciproques (statut Accepted) et qu'aucune relation Blocked n'existe entre eux.
        /// À utiliser avant toute lecture/envoi de message ou jonction de conversation.
        /// </summary>
        Task<bool> AreMutualFriendsAsyncService(Guid userId, Guid otherUserId);
    }
}
