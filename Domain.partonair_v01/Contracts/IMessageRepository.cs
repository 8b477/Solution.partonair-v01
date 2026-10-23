using Domain.partonair_v01.Entities;

namespace Domain.partonair_v01.Contracts
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<ICollection<Message>> GetConversationAsync(Guid firstUserId, Guid secondUserId, int take);
        Task<IDictionary<Guid, int>> CountUnreadBySenderAsync(Guid receiverId);
        Task<int> MarkConversationReadAsync(Guid receiverId, Guid senderId);
    }
}
