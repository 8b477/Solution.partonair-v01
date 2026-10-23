using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Interfaces
{
    public interface IMessageService
    {
        Task<MessageViewDTO> SendAsyncService(Guid senderId, Guid receiverId, string content);
        Task<ConversationViewDTO> GetConversationAsyncService(Guid userId, Guid otherUserId, int take);
        Task<ICollection<UnreadConversationDTO>> GetUnreadAsyncService(Guid userId);
        Task MarkConversationReadAsyncService(Guid userId, Guid otherUserId);
    }
}
