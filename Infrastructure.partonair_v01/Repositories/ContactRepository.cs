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
                                     .Where(c => c.ContactReceiverId == senderId && c.ContactSenderId ==  contactId)
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
    }
}
