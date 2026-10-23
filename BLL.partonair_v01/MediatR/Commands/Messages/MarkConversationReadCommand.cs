using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Messages
{
    public record MarkConversationReadCommand(Guid UserId, Guid OtherUserId) : IRequest;
}
