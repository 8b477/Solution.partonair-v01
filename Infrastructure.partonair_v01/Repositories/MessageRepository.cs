using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.partonair_v01.Repositories
{
    public class MessageRepository(ApplicationDbContext context) : GenericRepository<Message>(context), IMessageRepository
    {
        public async Task<ICollection<Message>> GetConversationAsync(Guid firstUserId, Guid secondUserId, int take)
        {
            return await _dbSet
                .Where(message =>
                    (message.SenderId == firstUserId && message.ReceiverId == secondUserId) ||
                    (message.SenderId == secondUserId && message.ReceiverId == firstUserId))
                .Include(message => message.Sender).ThenInclude(user => user.ProfileUser)
                .OrderByDescending(message => message.SentAt)
                .ThenByDescending(message => message.Id)
                .Take(take)
                .OrderBy(message => message.SentAt)
                .ThenBy(message => message.Id)
                .ToListAsync();
        }

        public async Task<IDictionary<Guid, int>> CountUnreadBySenderAsync(Guid receiverId)
        {
            return await _dbSet
                .Where(message => message.ReceiverId == receiverId && message.ReadAt == null)
                .GroupBy(message => message.SenderId)
                .Select(group => new { SenderId = group.Key, Count = group.Count() })
                .ToDictionaryAsync(row => row.SenderId, row => row.Count);
        }

        public async Task<int> MarkConversationReadAsync(Guid receiverId, Guid senderId)
        {
            var now = DateTime.UtcNow;

            return await _dbSet
                .Where(message => message.ReceiverId == receiverId && message.SenderId == senderId && message.ReadAt == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(message => message.ReadAt, now));
        }
    }
}
