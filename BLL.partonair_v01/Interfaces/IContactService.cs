using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Interfaces
{
    public interface IContactService
    {
        Task<ContactViewDTO> CreateAsyncService(ContactCreateDTO contact);
        Task DeleteAsyncService(Guid idSender, Guid idContact);

        Task<ICollection<ContactViewDTO>> GetAllAsyncService();
        Task<ICollection<ContactViewDTO>> GetAllPendingStatusAsyncService(string status);
        Task<ICollection<ContactViewDTO>> GetAllAcceptedStatusAsyncService(string status);
        Task<ICollection<ContactViewDTO>> GetAllBlockedStatusAsyncService(string status);
        Task<ContactViewDTO> GetByGuidAsyncService(Guid id);
        Task<ICollection<ContactViewDTO>> GetByNameAsyncService(string name);
        Task<ContactViewDTO> GetByEmailAsyncService(string email);

        Task<string> AcceptedRequestAsyncService(Guid idContact);
        Task<string> RefusedRequestAsyncService(Guid idContact);
        Task<string> LockContactRequestAsyncService(Guid idSender, UserToLock UserToLock);
        Task<string> UnlockContactRequestAsyncService(Guid idSender, UserToUnlock UserToUnlock);
    }
}
