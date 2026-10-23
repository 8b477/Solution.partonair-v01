using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Messages
{
    public record GetConversationQuery(Guid UserId, Guid OtherUserId, int Take) : IRequest<ConversationViewDTO>;
}
