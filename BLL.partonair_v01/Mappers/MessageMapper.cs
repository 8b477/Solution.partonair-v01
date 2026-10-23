using Domain.partonair_v01.Entities;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.Mappers
{
    public static class MessageMapper
    {
        internal static MessageViewDTO ToView(this Message entity)
        {
            return new MessageViewDTO(
                entity.Id,
                entity.SenderId,
                entity.ReceiverId,
                entity.Content,
                entity.SentAt,
                entity.ReadAt,
                entity.Sender?.Name ?? string.Empty,
                entity.Sender?.ProfileUser?.AvatarSeed);
        }
    }
}
