using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Commands.Messages
{
    public class SendMessageCommandHandler(IMessageService messageService) : IRequestHandler<SendMessageCommand, MessageViewDTO>
    {
        private readonly IMessageService _messageService = messageService;

        public async Task<MessageViewDTO> Handle(SendMessageCommand request, CancellationToken cancellationToken)
        {
            return await _messageService.SendAsyncService(request.SenderId, request.ReceiverId, request.Content);
        }
    }
}
