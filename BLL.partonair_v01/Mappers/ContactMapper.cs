using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Mappers
{
    public static class ContactMapper
    {
        internal static Contact ToPendingEntity(User receiver, User sender)
        {
            return new Contact
            {
                Id = Guid.NewGuid(),
                ContactReceiverId = receiver.Id,
                ContactSenderId = sender.Id,
                ContactMail = receiver.Mail,
                ContactName = receiver.Name,
                IsFriendly = false,
                IsBlocked = false,
                ContactReceiver = receiver,
                ContactSender = sender,
                ContactStatus = StatusContact.Pending
            };
        }

        internal static ContactViewDTO ToView(this Contact entity)
        {
            return new ContactViewDTO
            (
                entity.Id,
                entity.ContactSenderId,
                entity.ContactReceiverId,
                entity.ContactName,
                entity.ContactMail,
                entity.IsFriendly,
                entity.IsBlocked,
                entity.BlockedAt,
                entity.AcceptedAt,
                entity.ContactStatus.ToString()
            );
        }

        internal static MyContactViewDTO ToMyView(this Contact entity, User otherUser)
        {
            return new MyContactViewDTO(
                entity.Id,
                otherUser.Id,
                otherUser.Name,
                otherUser.Role.ToString(),
                entity.ContactStatus.ToString(),
                entity.AcceptedAt,
                entity.BlockedAt,
                otherUser.ProfileUser?.AvatarSeed,
                otherUser.ProfileUser?.Id);
        }


    }
}
