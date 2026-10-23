using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Messages
{
    public record SendMessageCommand(Guid SenderId, Guid ReceiverId, string Content) : IRequest<MessageViewDTO>;
}
