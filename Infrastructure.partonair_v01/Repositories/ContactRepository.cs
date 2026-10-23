using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Exceptions.Enums;
using Infrastructure.partonair_v01.ORM.EFCore.Settings;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.partonair_v01.Repositories
{
    public class ContactRepository(ApplicationDbContext ctx) : GenericRepository<Contact>(ctx), IContactRepository
    {
        public async Task<Contact> FindContactAsync(Guid senderId, Guid contactId)
        {
            var result = await _ctx.Contacts
                                     .Where(c => c.ContactSenderId == senderId && c.ContactReceiverId == contactId)
                                     .FirstOrDefaultAsync();

            return result ?? throw new InfrastructureLayerException(InfrastructureLayerErrorType.EntityIsNullException,$"Identifier sender : {senderId} or identifier receiver : {contactId} - No match");
        }

        public async Task<Contact> GetByEmailAsync(string email)
        {
            var result = await _ctx.Contacts
                                   .Where(c => c.ContactMail == email)
                                   .FirstOrDefaultAsync();

            return result ?? throw new InfrastructureLayerException(InfrastructureLayerErrorType.EntityIsNullException,$"Identifier {email} - No match");
        }

        public async Task<ICollection<Contact>> GetByNameAsync(string name)
        {
            var result = await _ctx.Contacts
                                   .Where(c => c.ContactName == name)
                                   .ToListAsync();

            return 
                result.Count == 0
                ? throw new InfrastructureLayerException(InfrastructureLayerErrorType.EntityIsNullException,$"identifier {name} - No match")
                : result;
        }

        public async Task<ICollection<Contact>> GetByPendingStatusAsync(StatusContact status)
        {
            var result = await _ctx.Contacts
                                   .Where(c => c.ContactStatus == status)
                                   .ToListAsync();

            return result ?? [];
        }

        public async Task<ICollection<Contact>> GetByAcceptedStatusAsync(StatusContact status)
        {
            var result = await _ctx.Contacts
                                   .Where(c => c.ContactStatus == status)
                                   .ToListAsync();

            return result ?? [];
        }

        public async Task<ICollection<Contact>> GetByBlockedStatusAsync(StatusContact status)
        {
            var result = await _ctx.Contacts
                                   .Where(c => c.ContactStatus == status)
                                   .ToListAsync();

            return result ?? [];
        }

        public async Task<bool> CheckIsContactExist(Guid idToCheck1, Guid idToCheck2)
        {
            var result1 = await _dbSet.Where(c => c.ContactSenderId == idToCheck1 && c.ContactReceiverId == idToCheck2).FirstOrDefaultAsync();
            var result2 = await _dbSet.Where(c => c.ContactReceiverId == idToCheck1 && c.ContactSenderId == idToCheck2).FirstOrDefaultAsync();

            if (result1 != null || result2 != null)
                return true;

            return false;
        }

        public async Task<ICollection<Contact>> GetPairAsync(Guid firstUserId, Guid secondUserId)
        {
            return await _dbSet
                .Where(contact =>
                    (contact.ContactSenderId == firstUserId && contact.ContactReceiverId == secondUserId) ||
                    (contact.ContactSenderId == secondUserId && contact.ContactReceiverId == firstUserId))
                .Include(contact => contact.ContactSender).ThenInclude(user => user.ProfileUser)
                .Include(contact => contact.ContactReceiver).ThenInclude(user => user.ProfileUser)
                .ToListAsync();
        }

        public async Task<ICollection<Contact>> GetByUserAndStatusAsync(Guid userId, StatusContact status)
        {
            return await _dbSet
                .Where(contact =>
                    (contact.ContactSenderId == userId || contact.ContactReceiverId == userId) &&
                    contact.ContactStatus == status)
                .Include(contact => contact.ContactSender).ThenInclude(user => user.ProfileUser)
                .Include(contact => contact.ContactReceiver).ThenInclude(user => user.ProfileUser)
                .ToListAsync();
        }

        public async Task<ICollection<Contact>> GetBySenderAndStatusAsync(Guid senderId, StatusContact status)
        {
            return await _dbSet
                .Where(contact => contact.ContactSenderId == senderId && contact.ContactStatus == status)
                .Include(contact => contact.ContactReceiver).ThenInclude(user => user.ProfileUser)
                .ToListAsync();
        }

        public async Task<ICollection<Contact>> GetByReceiverAndStatusAsync(Guid receiverId, StatusContact status)
        {
            return await _dbSet
                .Where(contact => contact.ContactReceiverId == receiverId && contact.ContactStatus == status)
                .Include(contact => contact.ContactSender).ThenInclude(user => user.ProfileUser)
                .ToListAsync();
        }

        public async Task<ICollection<Contact>> GetAllByReceiverAsync(Guid receiverId)
        {
            return await _dbSet
                .Where(contact => contact.ContactReceiverId == receiverId)
                .ToListAsync();
        }
    }
}
