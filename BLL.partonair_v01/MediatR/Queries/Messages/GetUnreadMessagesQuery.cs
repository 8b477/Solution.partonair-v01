using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Messages
{
    public record GetUnreadMessagesQuery(Guid UserId) : IRequest<ICollection<UnreadConversationDTO>>;
}
