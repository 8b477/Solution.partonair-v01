using SharedModels.partonair_v01.DTOS;

using BLL.partonair_v01.Mappers;
using BLL.partonair_v01.Interfaces;
using Domain.partonair_v01.Exceptions.Enums;
using Domain.partonair_v01.Contracts;
using Domain.partonair_v01.Exceptions;
using Domain.partonair_v01.Enums;
using Domain.partonair_v01.Entities;


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
        public async Task<ContactRequestResultDTO> CreateAsyncService(Guid senderId, Guid receiverId)
        {
            if (senderId == receiverId)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Tu ne peux pas t’ajouter toi-même.");

            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var sender = await _UOW.Users.GetByGuidAsync(senderId);
                var receiver = await _UOW.Users.GetByGuidAsync(receiverId);
                var pair = await _UOW.Contacts.GetPairAsync(senderId, receiverId);

                if (pair.Any(contact => contact.ContactStatus == StatusContact.Blocked))
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Une relation de blocage empêche cette demande.");

                if (pair.Count > 0)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Une demande ou une relation existe déjà pour ce profil.");

                var request = ContactMapper.ToPendingEntity(receiver, sender);
                var created = await _UOW.Contacts.CreateAsync(request);

                return new ContactRequestResultDTO(created.Id, created.ContactStatus.ToString());
            });
        }

        public async Task DeleteAsyncService(Guid idSender, Guid idContact)
        {
            await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var relationships = await _UOW.Contacts.GetPairAsync(idSender, idContact);
                var acceptedRelationships = relationships
                    .Where(contact => contact.ContactStatus == StatusContact.Accepted)
                    .ToList();

                if (acceptedRelationships.Count == 0)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.EntityIsNotExistingException, "Cette relation d’amitié est introuvable.");

                foreach (var relationship in acceptedRelationships)
                    await _UOW.Contacts.Delete(relationship.Id);
            });
        }

        public async Task<string> AcceptedRequestAsyncService(Guid userId, Guid idContact)
        {
            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var request = await _UOW.Contacts.GetByGuidAsync(idContact);
                if (request.ContactReceiverId != userId || request.ContactStatus != StatusContact.Pending)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.EntityIsNotExistingException, "Cette demande est introuvable ou ne t’est pas destinée.");

                var requester = await _UOW.Users.GetByGuidAsync(request.ContactSenderId);
                var receiver = await _UOW.Users.GetByGuidAsync(request.ContactReceiverId);
                var pair = await _UOW.Contacts.GetPairAsync(request.ContactSenderId, request.ContactReceiverId);

                if (pair.Any(contact => contact.ContactStatus == StatusContact.Blocked))
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Cette relation est bloquée et ne peut pas être acceptée.");

                var acceptedAt = DateTime.UtcNow;
                request.IsFriendly = true;
                request.ContactStatus = StatusContact.Accepted;
                request.AcceptedAt = acceptedAt;
                request.IsBlocked = false;

                var reciprocal = pair.FirstOrDefault(contact =>
                    contact.ContactSenderId == receiver.Id && contact.ContactReceiverId == requester.Id);

                if (reciprocal is null)
                {
                    await _UOW.Contacts.CreateAsync(new Contact
                    {
                        Id = Guid.NewGuid(),
                        ContactSenderId = receiver.Id,
                        ContactReceiverId = requester.Id,
                        ContactSender = receiver,
                        ContactReceiver = requester,
                        ContactName = requester.Name,
                        ContactMail = requester.Mail,
                        IsFriendly = true,
                        IsBlocked = false,
                        ContactStatus = StatusContact.Accepted,
                        AcceptedAt = acceptedAt
                    });
                }
                else
                {
                    reciprocal.ContactStatus = StatusContact.Accepted;
                    reciprocal.IsFriendly = true;
                    reciprocal.IsBlocked = false;
                    reciprocal.AcceptedAt = acceptedAt;
                }

                return "Demande acceptée.";
            });
        }

        public async Task<string> RefusedRequestAsyncService(Guid userId, Guid idContact)
        {
            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var request = await _UOW.Contacts.GetByGuidAsync(idContact);
                if (request.ContactReceiverId != userId || request.ContactStatus != StatusContact.Pending)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.EntityIsNotExistingException, "Cette demande est introuvable ou ne t’est pas destinée.");

                request.IsFriendly = false;
                request.ContactStatus = StatusContact.Refused;

                return "Demande refusée.";
            });
        }

        public async Task<string> LockContactRequestAsyncService(Guid idSender, UserToLock idUserToLock)
        {
            if (idSender == idUserToLock.Id_UserToLock)
                throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Tu ne peux pas bloquer ton propre profil.");

            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var sender = await _UOW.Users.GetByGuidAsync(idSender);
                var target = await _UOW.Users.GetByGuidAsync(idUserToLock.Id_UserToLock);
                var pair = await _UOW.Contacts.GetPairAsync(sender.Id, target.Id);
                var blockedRelationship = pair.FirstOrDefault(contact =>
                    contact.ContactSenderId == sender.Id && contact.ContactReceiverId == target.Id);
                var isNewRelationship = blockedRelationship is null;

                if (blockedRelationship?.ContactStatus == StatusContact.Blocked)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.ConstraintViolationErrorException, "Ce profil est déjà bloqué.");

                if (blockedRelationship is null)
                {
                    blockedRelationship = new Contact
                    {
                        Id = Guid.NewGuid(),
                        ContactSender = sender,
                        ContactReceiver = target,
                        ContactSenderId = sender.Id,
                        ContactReceiverId = target.Id,
                        ContactName = target.Name,
                        ContactMail = target.Mail
                    };
                }

                blockedRelationship.IsFriendly = false;
                blockedRelationship.ContactStatus = StatusContact.Blocked;
                blockedRelationship.BlockedAt = DateTime.UtcNow;
                blockedRelationship.IsBlocked = true;

                if (isNewRelationship)
                    await _UOW.Contacts.CreateAsync(blockedRelationship);

                var incomingRequest = pair.FirstOrDefault(contact =>
                    contact.ContactSenderId == target.Id && contact.ContactReceiverId == sender.Id &&
                    contact.ContactStatus == StatusContact.Pending);
                if (incomingRequest is not null)
                {
                    incomingRequest.ContactStatus = StatusContact.Refused;
                    incomingRequest.IsFriendly = false;
                }

                return "Profil bloqué.";
            });
        }

        public async Task<string> UnlockContactRequestAsyncService(Guid idSender, UserToUnlock userToUnlock)
        {
            return await _UOW.ExecuteInTransactionAsync(async () =>
            {
                var sender = await _UOW.Users.GetByGuidAsync(idSender);
                var target = await _UOW.Users.GetByGuidAsync(userToUnlock.Id_UserToUnlock);
                var pair = await _UOW.Contacts.GetPairAsync(sender.Id, target.Id);
                var blockedRelationship = pair.FirstOrDefault(contact =>
                    contact.ContactSenderId == sender.Id && contact.ContactReceiverId == target.Id &&
                    contact.ContactStatus == StatusContact.Blocked);

                if (blockedRelationship is null)
                    throw new ApplicationLayerException(ApplicationLayerErrorType.EntityIsNotExistingException, "Ce profil n’est pas bloqué.");

                var reciprocalFriendship = pair.FirstOrDefault(contact =>
                    contact.ContactSenderId == target.Id && contact.ContactReceiverId == sender.Id &&
                    contact.ContactStatus == StatusContact.Accepted);

                if (reciprocalFriendship is null)
                    await _UOW.Contacts.Delete(blockedRelationship.Id);
                else
                {
                    blockedRelationship.ContactStatus = StatusContact.Accepted;
                    blockedRelationship.IsFriendly = true;
                    blockedRelationship.IsBlocked = false;
                }

                return "Profil débloqué.";
            });
        }

        public async Task<bool> AreMutualFriendsAsyncService(Guid userId, Guid otherUserId)
        {
            if (userId == otherUserId)
                return false;

            var pair = await _UOW.Contacts.GetPairAsync(userId, otherUserId);

            if (pair.Count == 0 || pair.Any(contact => contact.ContactStatus == StatusContact.Blocked))
                return false;

            return pair.Any(contact => contact.ContactStatus == StatusContact.Accepted);
        }

        #endregion


        #region Queries

        public async Task<ICollection<MyContactViewDTO>> GetFriendsAsyncService(Guid userId)
        {
            var accepted = await _UOW.Contacts.GetByUserAndStatusAsync(userId, StatusContact.Accepted);
            var blocked = await _UOW.Contacts.GetByUserAndStatusAsync(userId, StatusContact.Blocked);
            var blockedUserIds = blocked
                .Select(contact => contact.ContactSenderId == userId ? contact.ContactReceiverId : contact.ContactSenderId)
                .ToHashSet();

            return accepted
                .Where(contact => !blockedUserIds.Contains(
                    contact.ContactSenderId == userId ? contact.ContactReceiverId : contact.ContactSenderId))
                .GroupBy(contact => contact.ContactSenderId == userId ? contact.ContactReceiverId : contact.ContactSenderId)
                .Select(group => group.First())
                .Select(contact => contact.ToMyView(
                    contact.ContactSenderId == userId ? contact.ContactReceiver : contact.ContactSender))
                .ToList();
        }

        /// <summary>
        /// Contacts affichés dans la messagerie : amis + relations bloquées (dans les deux sens),
        /// pour que la personne bloquée voie la conversation en lecture seule.
        /// Status vaut "Accepted", "BlockedByMe" ou "BlockedByOther".
        /// </summary>
        public async Task<ICollection<MyContactViewDTO>> GetConversationContactsAsyncService(Guid userId)
        {
            var friends = await GetFriendsAsyncService(userId);
            var blocked = await _UOW.Contacts.GetByUserAndStatusAsync(userId, StatusContact.Blocked);

            var blockedContacts = blocked
                .GroupBy(contact => contact.ContactSenderId == userId ? contact.ContactReceiverId : contact.ContactSenderId)
                .Select(group =>
                {
                    // Si les deux se sont bloqués, c'est mon propre blocage qui prime (je peux débloquer).
                    var mine = group.FirstOrDefault(contact => contact.ContactSenderId == userId);
                    var contact = mine ?? group.First();
                    var status = mine is not null ? RelationshipStatus.BlockedByMe : RelationshipStatus.BlockedByOther;

                    return contact.ToMyView(mine is not null ? contact.ContactReceiver : contact.ContactSender) with { Status = status };
                });

            return friends
                .Concat(blockedContacts)
                .OrderBy(contact => contact.UserName)
                .ToList();
        }

        public async Task<RelationshipDTO> GetRelationshipAsyncService(Guid userId, Guid otherUserId)
        {
            if (userId == otherUserId)
                return new RelationshipDTO(RelationshipStatus.None, null);

            var pair = await _UOW.Contacts.GetPairAsync(userId, otherUserId);
            var mine = pair.FirstOrDefault(contact => contact.ContactSenderId == userId);
            var theirs = pair.FirstOrDefault(contact => contact.ContactSenderId == otherUserId);

            if (mine?.ContactStatus == StatusContact.Blocked)
                return new RelationshipDTO(RelationshipStatus.BlockedByMe, null);
            if (theirs?.ContactStatus == StatusContact.Blocked)
                return new RelationshipDTO(RelationshipStatus.BlockedByOther, null);
            if (pair.Any(contact => contact.ContactStatus == StatusContact.Accepted))
                return new RelationshipDTO(RelationshipStatus.Friends, null);
            if (mine?.ContactStatus == StatusContact.Pending)
                return new RelationshipDTO(RelationshipStatus.PendingOutgoing, mine.Id);
            if (theirs?.ContactStatus == StatusContact.Pending)
                return new RelationshipDTO(RelationshipStatus.PendingIncoming, theirs.Id);
            if (pair.Count > 0)
                return new RelationshipDTO(RelationshipStatus.Refused, null);

            return new RelationshipDTO(RelationshipStatus.None, null);
        }

        public async Task<ICollection<MyContactViewDTO>> GetIncomingRequestsAsyncService(Guid userId)
        {
            var incoming = await _UOW.Contacts.GetByReceiverAndStatusAsync(userId, StatusContact.Pending);
            var blocked = await _UOW.Contacts.GetByUserAndStatusAsync(userId, StatusContact.Blocked);
            var blockedUserIds = blocked
                .Select(contact => contact.ContactSenderId == userId ? contact.ContactReceiverId : contact.ContactSenderId)
                .ToHashSet();

            return incoming
                .Where(contact => !blockedUserIds.Contains(contact.ContactSenderId))
                .Select(contact => contact.ToMyView(contact.ContactSender))
                .ToList();
        }

        public async Task<ICollection<MyContactViewDTO>> GetOutgoingRequestsAsyncService(Guid userId)
        {
            var outgoing = await _UOW.Contacts.GetBySenderAndStatusAsync(userId, StatusContact.Pending);
            var blocked = await _UOW.Contacts.GetByUserAndStatusAsync(userId, StatusContact.Blocked);
            var blockedUserIds = blocked
                .Select(contact => contact.ContactSenderId == userId ? contact.ContactReceiverId : contact.ContactSenderId)
                .ToHashSet();

            return outgoing
                .Where(contact => !blockedUserIds.Contains(contact.ContactReceiverId))
                .Select(contact => contact.ToMyView(contact.ContactReceiver))
                .ToList();
        }

        public async Task<ICollection<MyContactViewDTO>> GetBlockedContactsAsyncService(Guid userId)
        {
            var blocked = await _UOW.Contacts.GetBySenderAndStatusAsync(userId, StatusContact.Blocked);

            return blocked
                .Select(contact => contact.ToMyView(contact.ContactReceiver))
                .ToList();
        }

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
