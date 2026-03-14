using Domain.partonair_v01.Entities;
using Domain.partonair_v01.Enums;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Mappers
{
    public static class ContactMapper
    {
        internal static Contact ToEntity(this ContactCreateDTO dto, User receiver, User sender)
        {
            return new Contact
            {
                ContactReceiverId = receiver.Id, // User to add
                ContactSenderId = dto.Id_Sender, // User Sender
                ContactMail = receiver.Mail,
                ContactName = receiver.Name,
                AcceptedAt = DateTime.Now,
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


    }
}
