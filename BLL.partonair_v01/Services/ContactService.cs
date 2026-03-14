using SharedModels.partonair_v01.DTOS;

using BLL.partonair_v01.Mappers;
using BLL.partonair_v01.Interfaces;
using Domain.partonair_v01.Exceptions.Enums;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Enums;


namespace BLL.partonair_v01.Services
{
    public class ContactService(IUnitOfWork UOW) : IContactService
    {

        // <--------------------------------> TODO <-------------------------------->
        // Need to add notification when new contact is created -> 'CreateAsyncService()'
        // If one user send invitiation this is refused i need to controll make new request friend
        // Like add timer, when one invitation is refused, Unauthorize the re-send like one month
        // At this time the possibility to block this contact
        // <--------------------------------> **** <-------------------------------->

        private readonly IUnitOfWork _UOW = UOW;


        #region Commands
        public async Task<ContactViewDTO> CreateAsyncService(ContactCreateDTO contact)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var receiver = await _UOW.Users.GetByGuidAsync(contact.Id_Receiver);
                var sender = await _UOW.Users.GetByGuidAsync(contact.Id_Sender);
                var existingContact = await _UOW.Contacts.CheckIsContactExist(receiver.Id, sender.Id);
                
                if (existingContact)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "The contact you wish to add is already in the friends list, please check your request");

                var contactEntity = contact.ToEntity(receiver, sender);

                var result = await _UOW.Contacts.CreateAsync(contactEntity);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return result.ToView();
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteAsyncService(Guid idSender, Guid idContact)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var result1 = await _UOW.Contacts.FindContactAsync(idSender, idContact);
                var result2 = await _UOW.Contacts.FindContactAsync(idContact, idSender);

                await _UOW.Contacts.Delete(result1.Id);
                await _UOW.Contacts.Delete(result2.Id);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return;
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }
    
        public async Task<string> AcceptedRequestAsyncService(Guid idContact)
        {
            var existingContact = await _UOW.Contacts.GetByGuidAsync(idContact);

            existingContact.IsFriendly = true;
            existingContact.ContactStatus = StatusContact.Accepted;
            existingContact.AcceptedAt = DateTime.Now;

            await _UOW.Contacts.Update(existingContact);

            await _UOW.SaveChangesAsync();

            return "Request accepted !";
        }

        public async Task<string> RefusedRequestAsyncService(Guid idContact)
        {
            var result = await _UOW.Contacts.GetByGuidAsync(idContact);

            result.IsFriendly = false;
            result.ContactStatus = StatusContact.Refused;

            await _UOW.Contacts.Update(result);

            await _UOW.SaveChangesAsync();

            return "Request refused !";
        }

        public async Task<string> LockContactRequestAsyncService(Guid idSender, UserToLock idUserToLock)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var existingSender = await _UOW.Users.GetByGuidAsync(idSender);
                var existingUserToBlock = await _UOW.Users.GetByGuidAsync(idUserToLock.Id_UserToLock);

                var existingContact = await _UOW.Contacts.FindContactAsync(existingSender.Id, existingUserToBlock.Id);


                if (existingContact.IsBlocked == true)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, $"The user : '{existingUserToBlock.Name} is already blocked'");

                existingContact.IsFriendly = false;
                existingContact.ContactStatus = StatusContact.Blocked;
                existingContact.BlockedAt = DateTime.Now;
                existingContact.IsBlocked = true;

                var updated = await _UOW.Contacts.Update(existingContact);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return $"The User : '{existingUserToBlock.Name}' is now blocked. Identifier of request : {updated.Id}";
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<string> UnlockContactRequestAsyncService(Guid idSender, UserToUnlock userToUnlock)
        {
            try
            {
                await _UOW.BeginTransactionAsync();

                var existingSender = await _UOW.Users.GetByGuidAsync(idSender);
                var existingUserToUnlock = await _UOW.Users.GetByGuidAsync(userToUnlock.Id_UserToUnlock);

                var existingContact = await _UOW.Contacts.FindContactAsync(existingSender.Id, existingUserToUnlock.Id);

                if (existingContact.IsBlocked == false)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, $"The user : '{existingUserToUnlock.Name}' is not blocked'");

                existingContact.ContactStatus = StatusContact.Accepted;
                existingContact.IsFriendly = true;
                existingContact.IsBlocked = false;
                existingContact.AcceptedAt = existingContact.AcceptedAt ?? DateTime.Now;

                var updated = await _UOW.Contacts.Update(existingContact);

                await _UOW.SaveChangesAsync();
                await _UOW.CommitTransactionAsync();

                return $"The User : '{existingUserToUnlock.Name}' is now unlocked. Identifier of request : {updated.Id}";
            }
            catch
            {
                await _UOW.RollbackTransactionAsync();
                throw;
            }
        }

        #endregion


        #region Queries

        public async Task<ICollection<ContactViewDTO>> GetAllPendingStatusAsyncService(string status)
        {
            if (!IsValidPendingStatus(status))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "The valid status is 'Pending' - NO CASE SENSITIVE");

            var result = await _UOW.Contacts.GetByPendingStatusAsync(StatusContact.Pending);

            return result
                        .Select(c => c.ToView())
                        .ToList();
        }

        public async Task<ICollection<ContactViewDTO>> GetAllAcceptedStatusAsyncService(string status)
        {
            if (!IsValidAcceptedStatus(status))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "The valid status is 'Accepted' - NO CASE SENSITIVE");

            var result = await _UOW.Contacts.GetByAcceptedStatusAsync(StatusContact.Accepted);

            return result
                        .Select(c => c.ToView())
                        .ToList();
        }

        public async Task<ICollection<ContactViewDTO>> GetAllBlockedStatusAsyncService(string status)
        {
            if (!IsValidBlockedStatus(status))
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "The valid status is 'Blocked' - NO CASE SENSITIVE");

            var result = await _UOW.Contacts.GetByBlockedStatusAsync(StatusContact.Blocked);

            return result
                        .Select(c => c.ToView())
                        .ToList();
        }

        public async Task<ICollection<ContactViewDTO>> GetAllAsyncService()
        {
            var result = await _UOW.Contacts.GetAllAsync();

            return result.Select(c => c.ToView())
                         .ToList();
        }

        public async Task<ContactViewDTO> GetByEmailAsyncService(string email)
        {
            var result = await _UOW.Contacts.GetByEmailAsync(email);

            return result.ToView();
        }

        public async Task<ContactViewDTO> GetByGuidAsyncService(Guid id)
        {
            var result = await _UOW.Contacts.GetByGuidAsync(id);

            return result.ToView();
        }

        public async Task<ICollection<ContactViewDTO>> GetByNameAsyncService(string name)
        {
            var result = await _UOW.Contacts.GetByNameAsync(name);

            return result
                        .Select(c => c.ToView())
                        .ToList();
        }

        #endregion


        #region PRIVATE METHODS
        private static bool IsValidPendingStatus(string status)
        {
            if (!Enum.TryParse<StatusContact>(status, true, out var _))
                return false;

            if (!StatusContact.Pending.ToString().Equals(status, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

        private static bool IsValidAcceptedStatus(string status)
        {
            if (!Enum.TryParse<StatusContact>(status, true, out var _))
                return false;

            if (!StatusContact.Accepted.ToString().Equals(status, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }

        private static bool IsValidBlockedStatus(string status)
        {
            if (!Enum.TryParse<StatusContact>(status, true, out var _))
                return false;

            if (!StatusContact.Blocked.ToString().Equals(status, StringComparison.CurrentCultureIgnoreCase))
                return false;

            return true;
        }
        #endregion
    }
}
