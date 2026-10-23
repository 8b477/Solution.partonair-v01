using BLL.partonair_v01.Interfaces;
using MediatR;


namespace BLL.partonair_v01.MediatR.Commands.Messages
{
    public class MarkConversationReadCommandHandler(IMessageService messageService) : IRequestHandler<MarkConversationReadCommand>
    {
        private readonly IMessageService _messageService = messageService;

        public async Task Handle(MarkConversationReadCommand request, CancellationToken cancellationToken)
        {
            await _messageService.MarkConversationReadAsyncService(request.UserId, request.OtherUserId);
        }
    }
}
