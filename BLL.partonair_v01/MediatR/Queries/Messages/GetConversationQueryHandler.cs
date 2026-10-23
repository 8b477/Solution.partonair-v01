using BLL.partonair_v01.Interfaces;
using MediatR;
using SharedModels.partonair_v01.DTOS;


namespace BLL.partonair_v01.MediatR.Queries.Messages
{
    public class GetConversationQueryHandler(IMessageService messageService) : IRequestHandler<GetConversationQuery, ConversationViewDTO>
    {
        private readonly IMessageService _messageService = messageService;

        public async Task<ConversationViewDTO> Handle(GetConversationQuery request, CancellationToken cancellationToken)
        {
            return await _messageService.GetConversationAsyncService(request.UserId, request.OtherUserId, request.Take);
        }
    }
}
