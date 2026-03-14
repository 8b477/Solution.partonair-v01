using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;

namespace Domain.partonair_v01.Contracts
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<Contact> FindContactAsync(Guid senderId, Guid contactId);
        Task<Contact> GetByEmailAsync(string email);
        Task<ICollection<Contact>> GetByNameAsync(string name);
        Task<ICollection<Contact>> GetByPendingStatusAsync(StatusContact status);
        Task<ICollection<Contact>> GetByAcceptedStatusAsync(StatusContact status);
        Task<ICollection<Contact>> GetByBlockedStatusAsync(StatusContact status);
        Task<bool> CheckIsContactExist(Guid idToCheck1, Guid idToCheck2);
    }
}
